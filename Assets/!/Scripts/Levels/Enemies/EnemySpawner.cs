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
    [HideInInspector] public int NumberOfEnemies { get; private set; }
    [HideInInspector] public int WaveNumber = 0;
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
        _state = SpawnerState.DuringFight;
    } 

    private void duringFight()
    {
        if(NumberOfEnemies == 0 && Waves.Count == WaveNumber)
        {
            Debug.Log("Last wave has ended");
            _state = SpawnerState.TriggerEnts;
            return;
        }

        if(NumberOfEnemies != 0) return;

        WaveInfo currentWave = Waves[WaveNumber];
        WaveNumber++;

        int newNumberOfEnemies = UnityEngine.Random.Range(currentWave.MinEnemies, currentWave.MaxEnemies + 1);

        Debug.Log($"Number of all enemies enemies: {newNumberOfEnemies}");
        Debug.Log($"=== Wave {WaveNumber} ===");

        var currentEnemies = currentWave.EnemyInfo
            .Where(x => x.minInfluence <= GameManager.Instance.TotalInfluence)
            .Where(x => x.maxInfluence >= GameManager.Instance.TotalInfluence)
            .ToList();

        if(currentEnemies.Count == 0) {
            Debug.LogWarning("No Enemies in wave");
            return;
        }

        NumberOfEnemies = newNumberOfEnemies;

        for(int i = 0; i < NumberOfEnemies; i++)
        {
            List<Transform> randomSpawnPoints = FluffyUtils.ShuffleList(SpawnPoints)
                .Select(x=>x)
                .Take(MaxExitsAtOnce)
                .ToList();

            int batch = i / randomSpawnPoints.Count;
            Transform spawnPoint = randomSpawnPoints[i%randomSpawnPoints.Count];

            int randomIndex = UnityEngine.Random.Range(0, currentEnemies.Count);
            EnemySpawnInfo enemy = currentEnemies[randomIndex];

            float firstBatchDelay = 3f;
            float delayBetweenEach = 1f;

            // Total delay = initial delay + time based on batch index
            float totalSmokeDelay = firstBatchDelay + (batch * delayBetweenEach);

            StartCoroutine(spawnEnemy(enemy.EnemyPrefab, spawnPoint.position, totalSmokeDelay, delayBetweenEach));
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

    private IEnumerator spawnEnemy(GameObject enemy, Vector3 position, float smokeDelay, float enemySpawnDelay)
    {
        yield return new WaitForSeconds(smokeDelay);

        Vector3 smokePosition = position;
        smokePosition.y += 1.5f;
        EnemySpawnExit smoke = Instantiate(_enemySpawnerExitPrefab, smokePosition, Quaternion.identity);
        smoke.SetDestroyTimer(enemySpawnDelay);

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
