using System.Collections;
using UnityEngine;

public class VektharBossBattleManager : MonoBehaviour
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
    public Transform VektharStart;
    [SerializeField] private VektharBoss _vektharPrefab;
    public ArenaWall[] walls;
    [HideInInspector] public VektharBoss Vekthar;

    // Previous player camera settings
    private float _previousMaxCameraDistance;
    private float _previousMinCameraDistance;

    void Start() {
        Player.Instance.gameObject.SetActive(false);
        
        AudioManager.Instance.setMusicArea(MusicArea.VEKTHAR);
        Player.Instance.SetPlayerPosition(LevelStart.position);
        Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
        Player.Instance.MaxCameraDistance = 25;
        Player.Instance.ResetToDefault();

        Player.Instance.gameObject.SetActive(true);
        
        CameraManager.SwitchCamera(Player.Instance.TopDownCamera);

        Invoke(nameof(sceneReady), 0.2f);
    }

    void sceneReady() {
        Player.Instance.SetPlayerPosition(LevelStart.position);
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
        Debug.Log("Player entered the arena. Starting Vek'thar boss battle!");
        _state = BattleState.StartOfBattle;

        GetComponent<Collider>().enabled = false;
    }

    private void beforeBattle() {
        
    }

    public IEnumerator disableSounds() {
        yield return new WaitForSeconds(30);

        GameManager.Instance.ChangeDimension(Dimension.HUB);

        // GameManager.Instance.GetComponent<AudioSource>().mute = false;
    }

    private void startOfBattle() {
        foreach(ArenaWall w in walls) {
            w.Switch();
        }

        Vekthar = Instantiate(_vektharPrefab, VektharStart.position, Quaternion.Euler(0, 45, 0));
        Player.UICanvas.HUDCanvas.ShowBossBar(Vekthar.LivingEntity);

        Vekthar.LivingEntity.OnDeath.AddListener(() => {
            Player.UICanvas.ChangeUITopState(UITopState.VideoPlayer);
            StartCoroutine(disableSounds());
        });

        _previousMinCameraDistance = Player.Instance.MinCameraDistance;
        _previousMaxCameraDistance = Player.Instance.MaxCameraDistance;
        Player.Instance.CameraDistance = FightCameraDistance;
        Player.Instance.MaxCameraDistance = FightCameraDistance;
        // Player.Instance.MinCameraDistance = FightCameraDistance;

        _state = BattleState.DuringBattle;
    }

    private void duringBattle() {

    }

    private void endOfBattle() {
        foreach(ArenaWall w in walls) {
            w.Switch();
        }

        Player.Instance.CameraDistance = _previousMaxCameraDistance;
        Player.Instance.MaxCameraDistance = _previousMaxCameraDistance;
        Player.Instance.MinCameraDistance = _previousMinCameraDistance;

        _state = BattleState.AfterBattle;
    }
}
