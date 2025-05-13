using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Location))]
[RequireComponent(typeof(Collider))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Wall _wallPrefab;
    [SerializeField] private EnemySpawnExit _enemySpawnerExitPrefab;
    [SerializeField] private EntAIController _entPrefab;
    public int MaxExitsAtOnce = 1;
    public int MaxNumberOfWaves = 1;
    public int MinNumberOfWaves = 1;
    public List<WaveInfo> Waves;
    enum SpawnerState
    {
        BeforeFight,
        StartingFight,
        DuringFight,
        TriggerEnts,
        DuringFightWithEnts,
        FightEnded,
    }

    private SpawnerState _state = SpawnerState.BeforeFight;

    public Transform SpawnPointsParent;

    private Location _location;
    private List<Wall> _activeWalls = new();

    private GameObject _enemiesParent;
    private List<LivingEntity> _enemies = new List<LivingEntity>();
    private List<EntAIController> _ents = new List<EntAIController>();

    [HideInInspector] public List<Transform> SpawnPoints;

    private int _waveNumber = 0;
    private int _numberOfWaves = 0;
    private bool _hasSpawned = false;

    public UnityEvent DefeatedEnemies;

    void Awake()
    {
        _location = GetComponent<Location>();
    }

    void Start()
    {
        foreach(Transform child in SpawnPointsParent) SpawnPoints.Add(child);

        if(MaxExitsAtOnce > SpawnPoints.Count)
        {
            Debug.LogWarning($"Maximum number of exit locations used at once was set to {MaxExitsAtOnce}, but number of them is only {SpawnPoints.Count}.");
            MaxExitsAtOnce = SpawnPoints.Count;
        }

        if(transform.Find("Enemies") == null)
        {
            _enemiesParent = new GameObject("Enemies");
            _enemiesParent.transform.parent = transform;
        }

        Transform trees = transform.Find("Trees");
        float chance = 0.1f; // 10 %
        if(trees != null)
        {   
            foreach(Transform t in trees)
            {
                if(UnityEngine.Random.Range(0f, 1f) > chance) continue;
                
                Transform treeTransform = t.transform;
                Destroy(t.gameObject);

                EntAIController newEnt = Instantiate(_entPrefab, treeTransform.position, Quaternion.identity, _enemiesParent.transform);
                newEnt.GetComponent<LivingEntity>().OnDeath.AddListener(() => {
                    _ents.Remove(newEnt);
                });

                _ents.Add(newEnt);

                Debug.Log($"New ent was spawned at position: {newEnt.transform.position}.");
            }
        }
        else
        {
            Debug.Log("No \"Trees\" child was found. Cannot spawn ents.");
        }
    }

    void Update()
    {
        switch(_state)
        {
            case SpawnerState.BeforeFight: { break; }
            case SpawnerState.StartingFight: { startFight(); break; }
            case SpawnerState.DuringFight: { duringFight(); break; }
            case SpawnerState.TriggerEnts: {
                if(_ents.Count > 0)
                {   
                    Debug.Log("Last wave has been defeated, triggering ents.");
                    _ents.ForEach(e => e.TriggerEndgame());
                    _state = SpawnerState.DuringFightWithEnts;
                }
                else
                {
                    Debug.Log("Last wave has been defeated and there are no more ents to trigger.");
                    _state = SpawnerState.FightEnded;
                }

                break; 
            }
            case SpawnerState.DuringFightWithEnts: { duringFightWithEnts(); break; }
            case SpawnerState.FightEnded: { fightEnded(); break; }
        }
    }

    private void startFight()
    {
        Debug.Log("Starting fight");

        spawnWalls();
        VektharControlManager.Instance.StartControlManager();

        _numberOfWaves = UnityEngine.Random.Range(MinNumberOfWaves, MaxNumberOfWaves + 1);
        _waveNumber = 0;

        _state = SpawnerState.DuringFight;
   
        Debug.Log($"Waves count: {_numberOfWaves}");
    } 

    private void duringFight()
    {
        if(!_hasSpawned && _waveNumber != 0) return;

        if(_enemies.Count == 0 && _numberOfWaves == _waveNumber)
        {
            Debug.Log("Last wave has ended");
            _state = SpawnerState.TriggerEnts;
            return;
        }

        if(_enemies.Count > 0 && hasOnlyFriendlyEnemies()) {
            Debug.Log($"Wave {_waveNumber} has only friendly enemies: {_enemies.Count}");

            foreach(LivingEntity enemy in _enemies) {
                enemy.Guild = GameManager.Instance.EnemyGuild;
                enemy.AvoidGuildChange = true;
                
                Charm enemyCharm = enemy.GetComponent<Charm>();
                if(enemyCharm != null) {
                    enemyCharm.Stop();
                }
            }
        }

        if(_enemies.Count != 0) return;

        Debug.Log("=== Starting new wave ===");

        float influence = GameManager.Instance.TotalInfluence;

        List<WaveInfo> shuffledWaves = FluffyUtils.ShuffleList(Waves);
        WaveInfo currentWave = shuffledWaves
            .Where(x => x.MinInfluence <= influence && x.MaxInfluence >= influence)
            .First();
        _waveNumber++;

        Debug.Log($"Number of enemies: {currentWave.EnemyPrefabs.Count()}");

        _hasSpawned = false;

        float firstBatchDelay = 3f;
        float delayBetweenEach = 1f;

        for(int i = 0; i < currentWave.EnemyPrefabs.Count(); i++)
        {
            List<Transform> randomSpawnPoints = FluffyUtils.ShuffleList(SpawnPoints)
                .Select(x=>x)
                .Take(MaxExitsAtOnce)
                .ToList();

            GameObject enemy = currentWave.EnemyPrefabs[i];

            int batch = i / randomSpawnPoints.Count;
            Transform spawnPoint = randomSpawnPoints[i%randomSpawnPoints.Count];

            // Total delay = initial delay + time based on batch index
            float totalSmokeDelay = firstBatchDelay + (batch * delayBetweenEach);

            StartCoroutine(spawnEnemy(enemy, spawnPoint.position, totalSmokeDelay, delayBetweenEach));
        }

        StartCoroutine(setToSpawned(firstBatchDelay + ((currentWave.EnemyPrefabs.Count() - 1) / Math.Min(SpawnPoints.Count, MaxExitsAtOnce) * delayBetweenEach) + delayBetweenEach + 1));
    }

    private void duringFightWithEnts()
    {
        if(_ents.Count == 0)
        {
            _state = SpawnerState.FightEnded;
        }
    }

    private void fightEnded()
    {
        Debug.Log("Fight has ended");
        removeWalls();
        DefeatedEnemies.Invoke();
        Destroy(this);
        VektharControlManager.Instance.StopControlManager();
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log($"Player entered new location at {other.transform.position}. Starting the fight!");

        GetComponent<Collider>().enabled = false;
        _state = SpawnerState.StartingFight;
    }

    private IEnumerator setToSpawned(float delay) {
        yield return new WaitForSeconds(delay);
        _hasSpawned = true;
        Debug.Log("Set to spawned after delay " + delay);
    }

    private IEnumerator spawnEnemy(GameObject enemy, Vector3 position, float smokeDelay, float enemySpawnDelay)
    {
        Debug.Log($"Spawning enemy after {smokeDelay + enemySpawnDelay} seconds");

        yield return new WaitForSeconds(smokeDelay);

        Vector3 smokePosition = position;
        smokePosition.y += 1.5f;
        EnemySpawnExit smoke = Instantiate(_enemySpawnerExitPrefab, smokePosition, Quaternion.identity);
        smoke.SetDestroyTimer(enemySpawnDelay);

        yield return new WaitForSeconds(enemySpawnDelay);

        GameObject newEnemy = Instantiate(enemy, position, Quaternion.identity, _enemiesParent.transform);

        Debug.Log($"Spawned new enemy at: {newEnemy.transform.position}");

        LivingEntity newEnemyEntity = newEnemy.GetComponent<LivingEntity>();

        newEnemyEntity.OnDeath.AddListener(() => {
            _enemies.Remove(newEnemyEntity);
        });

        _enemies.Add(newEnemyEntity);
        Debug.Log("Added new enemy to list");
        Debug.Log($"Enemies count: {_enemies.Count}");
    }

    private void spawnWalls()
    {
        Vector3 a = _location.GetTopLeftCorner3();
        Vector3 b = _location.GetTopRightCorner3();
        Vector3 c = _location.GetBottomRightCorner3();
        Vector3 d = _location.GetBottomLeftCorner3();

        Wall newWall;

        newWall = Instantiate(_wallPrefab, Vector3.zero, Quaternion.identity);
        newWall.PlaceWall(_location, a, b);
        _activeWalls.Add(newWall);

        newWall = Instantiate(_wallPrefab, Vector3.zero, Quaternion.identity);
        newWall.PlaceWall(_location, b, c);
        _activeWalls.Add(newWall);

        newWall = Instantiate(_wallPrefab, Vector3.zero, Quaternion.identity);
        newWall.PlaceWall(_location, c, d);
        _activeWalls.Add(newWall);

        newWall = Instantiate(_wallPrefab, Vector3.zero, Quaternion.identity);
        newWall.PlaceWall(_location, d, a);
        _activeWalls.Add(newWall);
        Debug.Log("Placed walls");
    }

    private void removeWalls()
    {
        foreach(var w in _activeWalls)
        {
            w.RemoveWall();
        }
        _activeWalls.Clear();
        Debug.Log("Removed walls");
    }

    private bool hasOnlyFriendlyEnemies() {
        foreach(LivingEntity enemy in _enemies) {
            if(enemy.Guild != Player.LivingEntity.Guild && !enemy.IsInvisible) {
                return false;
            }
        }

        return true;
    }
}
