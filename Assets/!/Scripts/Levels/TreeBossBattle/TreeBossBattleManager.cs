using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBossBattleManager : MonoBehaviour
{
    enum BattleState {
        BeforeBattle,
        StartOfBattle,
        DuringBattle,
        EndOfBattle,
        AfterBattle
    }

    [Header("Variables")]
    private BattleState _state = BattleState.BeforeBattle;
    public float FightCameraDistance;

    [Header("Game objects and prefabs")]
    public Transform LevelStart;
    public Transform TreeBossStart;
    [SerializeField] private TreeBossController _treeBossPrefab;
    public ArenaWall[] walls;
    [HideInInspector] public TreeBossController TreeBoss;

    public EntAIController GuardianPrefab;
    public List<Transform> GuardianSpawnPoints;
    public List<EntAIController> Guardians;
    public int MaxGuardians = 1;

    public EntAIController MinionPrefab;
    public List<Transform> MinionSpawnPoints;
    public List<EntAIController> Minions;

    public int MaxMinions = 4;
    public int MinWaveMinionsSpawned = 0;
    public int MaxWaveMinionsSpawned = 3;

    public float LastSpawnTime;

    public float DelayBetweenMiniEntSpawns = 1f;

    void Start() {
        Player.Instance.gameObject.SetActive(false);
        
        Player.Instance.SetPlayerPosition(LevelStart.position);
        Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
        Player.Instance.MaxCameraDistance = 25;
        Player.Instance.ResetToDefault();

        Player.Instance.gameObject.SetActive(true);
        
        CameraManager.SwitchCamera(Player.Instance.TopDownCamera);

        Invoke(nameof(sceneReady), 0.2f);
    }

    void sceneReady() {
        EventBus.SceneReadyEvent?.Invoke();
    }

    void Update() {
        switch (_state)
        {
            case BattleState.BeforeBattle:
                break;
            case BattleState.StartOfBattle:
                startOfBattle();
                break;
            case BattleState.DuringBattle:
                duringBattle();
                break;
            case BattleState.EndOfBattle:
                endOfBattle();
                break;
            case BattleState.AfterBattle:
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Player entered the arena. Starting Tree boss battle!");
        _state = BattleState.StartOfBattle;

        GetComponent<Collider>().enabled = false;
    }

    private void beforeBattle() {
        
    }

    public IEnumerator disableSounds() {
        yield return new WaitForSeconds(1);

        GameManager.Instance.GetComponent<AudioSource>().mute = true;
    
        yield return new WaitForSeconds(30);

        GameManager.Instance.GetComponent<AudioSource>().mute = false;
    }

    private void startOfBattle() {
        foreach(ArenaWall w in walls) {
            w.Switch();
        }

        TreeBoss = Instantiate(_treeBossPrefab, TreeBossStart.position, Quaternion.Euler(0, 225, 0));

        TreeBoss.GetComponent<LivingEntity>().OnDeath.AddListener(() => {
            Player.UICanvas.ChangeUITopState(UITopState.VideoPlayer);
            StartCoroutine(disableSounds());
        });

        CameraManager.ShakeCamera(3, 3);

        _state = BattleState.DuringBattle;

        Player.UICanvas.HUDCanvas.ShowBossBar(TreeBoss.GetComponent<LivingEntity>());
    }

    private void duringBattle() {
        if(Time.time - LastSpawnTime < 30) return;
        if(Minions.Count >= MaxMinions) return;
     
        Debug.Log("Spawning mini-ents");

        LastSpawnTime = Time.time;
        StartCoroutine(spawnMiniEnts(Random.Range(MinWaveMinionsSpawned, MaxWaveMinionsSpawned)));
    }

    private IEnumerator spawnMiniEnts(int count) {
        for(int i = 0; i < count; i++) {
            yield return new WaitForSeconds(DelayBetweenMiniEntSpawns);

            int x = Random.Range(0, MinionSpawnPoints.Count);
            Transform spawnPoint = MinionSpawnPoints[x];
            EntAIController ent = Instantiate(MinionPrefab, spawnPoint.position, Quaternion.Euler(0, Random.Range(0, 360), 0));

            Minions.Add(ent);

            ent.GetComponent<LivingEntity>().OnDeath.AddListener(() => {
                Minions.Remove(ent);
            });
        }

        if(Guardians.Count < MaxGuardians) {
            if(Random.Range(0f, 1f) > 0.5) {
                Debug.Log("Spawning guardian");

                CameraManager.ShakeCamera(3f, 2f);

                int x = Random.Range(0, GuardianSpawnPoints.Count);
                Transform spawnPoint = GuardianSpawnPoints[x];
                EntAIController ent = Instantiate(GuardianPrefab, spawnPoint.position, Quaternion.Euler(0, Random.Range(0, 360), 0));

                Guardians.Add(ent);

                ent.GetComponent<LivingEntity>().OnDeath.AddListener(() => {
                    Guardians.Remove(ent);
                });
            }
        }
    }

    private void endOfBattle() {
        foreach(ArenaWall w in walls) {
            w.Switch();
        }

        _state = BattleState.AfterBattle;
    }
}
