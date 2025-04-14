using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Location))]
[RequireComponent(typeof(Collider))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Wall _wallPrefab;
    [SerializeField] private EnemySpawnExit _enemySpawnerExitPrefab;
    public int MaxExitsAtOnce = 1;

    enum SpawnerState
    {
        BeforeFight,
        StartingFight,
        DuringFight,
        FightEnded,
    }
    private SpawnerState _state = SpawnerState.BeforeFight;
    public Transform SpawnPointsParent;
    [HideInInspector] public List<Transform> SpawnPoints;
    private Location _location;
    private List<Wall> _activeWalls = new();
    public List<WaveInfo> Waves;
    [HideInInspector] public int WaveNumber = 0;
    [HideInInspector] public int EnemyCount = 0;
    void Awake()
    {
        _location = GetComponent<Location>();
    }

    void Start()
    {
        foreach(Transform child in SpawnPointsParent) SpawnPoints.Add(child);
        if(MaxExitsAtOnce > SpawnPoints.Count)
        {
            Debug.LogWarning($"Maximum number of exit locations used at once was set to {MaxExitsAtOnce}, but number of them is only {SpawnPoints.Count}");
            MaxExitsAtOnce = SpawnPoints.Count;
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
        if(EnemyCount == 0 && Waves.Count == WaveNumber)
        {
            Debug.Log("Fight has ended");
            _state = SpawnerState.FightEnded;
            return;
        }

        if(EnemyCount != 0) return;

        WaveInfo currentWave = Waves[WaveNumber];
        WaveNumber++;

        int numberOfEnemies = UnityEngine.Random.Range(currentWave.MinEnemies, currentWave.MaxEnemies + 1);

        Debug.Log($"Number of all enemies enemies: {numberOfEnemies}");
        Debug.Log($"=== Wave {WaveNumber} ===");

        for(int i = 0; i < numberOfEnemies; i++)
        {
            List<Transform> randomSpawnPoints = FluffyUtils.ShuffleList(SpawnPoints)
                .Select(x=>x)
                .Take(MaxExitsAtOnce)
                .ToList();

            int batch = i / randomSpawnPoints.Count;
            Transform spawnPoint = randomSpawnPoints[i%randomSpawnPoints.Count];

            int randomIndex = UnityEngine.Random.Range(0, currentWave.EnemyInfo.Count);
            EnemySpawnInfo enemy = currentWave.EnemyInfo[randomIndex];

            int firstEnemiesDelay = 3;
            int delayBetweenSpawns = 1;
            
            if(batch != 0)
            {
                StartCoroutine(spawnEnemy(enemy.EnemyPrefab, spawnPoint.position, (delayBetweenSpawns*batch)-1 + firstEnemiesDelay, delayBetweenSpawns));
            }
            else
            {
                StartCoroutine(spawnEnemy(enemy.EnemyPrefab, spawnPoint.position, 0, firstEnemiesDelay));
            }
        }

        EnemyCount = numberOfEnemies;
    }

    private void fightEnded()
    {
        removeWalls();
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Player entered new location. Starting the fight!");
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

        GameObject newEnemy = Instantiate(enemy, position, Quaternion.identity);
        Debug.Log($"Spawned new enemy at: {newEnemy.transform.position}");
        newEnemy.GetComponent<LivingEntity>().OnDeath.AddListener(enemyIsKilled);
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

    private void enemyIsKilled()
    {
        EnemyCount--;
    }
}
