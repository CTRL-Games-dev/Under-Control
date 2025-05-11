using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    private GameObject _enemies;
    [HideInInspector] public List<Transform> SpawnPoints;
    private List<Transform> _usedSpawnPoints;
    [HideInInspector] public int NumberOfEnemies { get; private set; }
    private int _waveNumber = 0;
    private int _numberOfWaves = 0;
    public UnityEvent DefeatedEnemies;
    void Awake()
    {
        _location = GetComponent<Location>();
    }

    void Start()
    {

        _usedSpawnPoints = new();
        foreach(Transform child in SpawnPointsParent) SpawnPoints.Add(child);
        if(MaxExitsAtOnce > SpawnPoints.Count)
        {
            Debug.LogWarning($"Maximum number of exit locations used at once was set to {MaxExitsAtOnce}, but number of them is only {SpawnPoints.Count}.");
            MaxExitsAtOnce = SpawnPoints.Count;
        }

        if(transform.Find("Enemies") == null)
        {
            _enemies = new GameObject("Enemies");
            _enemies.transform.parent = transform;
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

                EntAIController newEnt = Instantiate(_entPrefab, treeTransform.position, Quaternion.identity);
                newEnt.transform.parent = _enemies.transform;

                Debug.Log($"New ent was spawned at position: {newEnt.transform.position}.");
            }
        }
        else
        {
            Debug.Log("No \"Trees\" child was found. Cannot spawn ents.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(_state)
        {
            case SpawnerState.BeforeFight: { break; }
            case SpawnerState.StartingFight: { startFight(); break; }
            case SpawnerState.DuringFight: { duringFight(); break; }
            case SpawnerState.TriggerEnts: {
                if(GetEntCount() > 0)
                {   
                    List<EntAIController> ents = GetEnts();
                    Debug.Log("Last wave has been defeated, triggering ents.");
                    ents.ForEach(e => e.TriggerEndgame());
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
        spawnWalls();

        _numberOfWaves = UnityEngine.Random.Range(MinNumberOfWaves, MaxNumberOfWaves + 1);
        _waveNumber = 0;

        _state = SpawnerState.DuringFight;
    } 

    private void duringFight()
    {
        if(NumberOfEnemies == 0 && _numberOfWaves == _waveNumber)
        {
            Debug.Log("Last wave has ended");
            _state = SpawnerState.TriggerEnts;
            return;
        }

        if(NumberOfEnemies != 0) return;

        Debug.Log("=== Starting new wave ===");

        float influence = GameManager.Instance.TotalInfluence;

        List<WaveInfo> shuffledWaves = FluffyUtils.ShuffleList(Waves);
        WaveInfo currentWave = shuffledWaves
            .Where(x => x.MinInfluence <= influence && x.MaxInfluence >= influence)
            .First();
        _waveNumber++;

        Debug.Log($"Number of enemies: {currentWave.EnemyPrefabs.Count()}");
        NumberOfEnemies = currentWave.EnemyPrefabs.Count();

        int previousBatch = 0;
        List<Transform> randomSpawnPoints = FluffyUtils.ShuffleList(SpawnPoints)
                    .Select(x=>x)
                    .Take(MaxExitsAtOnce)
                    .ToList();

        for(int i = 0; i < currentWave.EnemyPrefabs.Count(); i++)
        {
            GameObject enemy = currentWave.EnemyPrefabs[i];

            int batch = i / SpawnPoints.Count;

            if(previousBatch > batch) {
                randomSpawnPoints = FluffyUtils.ShuffleList(SpawnPoints)
                    .Select(x=>x)
                    .Take(MaxExitsAtOnce)
                    .ToList();
            }

            Transform spawnPoint = randomSpawnPoints[i%randomSpawnPoints.Count];

            if(_usedSpawnPoints.Count(x => x == spawnPoint) != 0 )
                _usedSpawnPoints.Add(spawnPoint);

            randomSpawnPoints.RemoveAt(i%randomSpawnPoints.Count);

            float firstBatchDelay = 3f;
            float spawnTime = 3f;
            float delayBetweenEach = 0.5f;

            // Total delay = initial delay + time based on batch index + some additional delay
            float totalSmokeDelay = firstBatchDelay + (batch * spawnTime + delayBetweenEach) + 0.2f * i;

            StartCoroutine(spawnEnemy(enemy, spawnPoint.position, totalSmokeDelay, delayBetweenEach, delayBetweenEach));
        }
    }

    private void duringFightWithEnts()
    {
        if(GetEntCount() == 0)
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
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log($"Player entered new location at {other.transform.position}. Starting the fight!");

        GetComponent<Collider>().enabled = false;
        _state = SpawnerState.StartingFight;
    }

    private IEnumerator spawnEnemy(GameObject enemy, Vector3 position, float smokeDelay, float enemySpawnDelay, float smokeFade)
    {
        yield return new WaitForSeconds(smokeDelay);

        Vector3 smokePosition = position;
        smokePosition.y += 1.5f;
        EnemySpawnExit smoke = Instantiate(_enemySpawnerExitPrefab, smokePosition, Quaternion.identity);

        smoke.SetDestroyTimer(enemySpawnDelay + smokeFade);

        yield return new WaitForSeconds(enemySpawnDelay);

        GameObject newEnemy = Instantiate(enemy, position, Quaternion.identity, _enemies.transform);
        newEnemy.transform.parent = _enemies.transform;
        Debug.Log($"Spawned new enemy at: {newEnemy.transform.position}");
        newEnemy.GetComponent<LivingEntity>().OnDeath.AddListener(enemyDied);
    }

    private void enemyDied()
    {
        NumberOfEnemies--;
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

    public int GetAllEnemiesCount()
    {
        return _enemies.transform.childCount;
    }

    public int GetEntCount()
    {
        int enemyCount = 0;
        foreach(Transform e in _enemies.transform)
        {
            if(e.tag == "Ent") enemyCount++;
        }
        return enemyCount;
    }

    // public int GetEnemiesCountWithoutEnts()
    // {
    //     int enemyCount = 0;
    //     foreach(Transform e in _enemies.transform)
    //     {
    //         if(e.tag != "Ent") enemyCount++;
    //     }
    //     return enemyCount;
    // }

    public List<EntAIController> GetEnts()
    {
        List<EntAIController> ents = new();
        foreach(Transform e in _enemies.transform)
        {
            if(e.tag == "Ent") {
                EntAIController entController = e.gameObject.GetComponent<EntAIController>();
                if(entController != null) ents.Add(entController);
            }
        }
        return ents;
    }
}
