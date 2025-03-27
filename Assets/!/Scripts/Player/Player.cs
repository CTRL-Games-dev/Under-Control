using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    #region Singleton
    public static Player Instance;
    [SerializeField] private UICanvas _uiCanvas;
    public static UICanvas UICanvas => Player.Instance._uiCanvas;
    private static RunStats _runStats;
    public static RunStats Stats {
        get {
            if (_runStats == null) {
                _runStats = Player.Instance.gameObject.GetComponent<RunStats>();
            }

            return _runStats;
        }
    }
    private static PlayerController _playerController;
    public static PlayerController PlayerController {
        get {
            if (_playerController == null) {
                _playerController = Player.Instance.gameObject.GetComponent<PlayerController>();
            }

            return _playerController;
        }
    } 
    private static LivingEntity _livingEntity;
    public static LivingEntity LivingEntity {
        get {
            if (_livingEntity == null) {
                _livingEntity = Player.Instance.gameObject.GetComponent<LivingEntity>();
            }

            return _livingEntity;
        }
    }
    public static HumanoidInventory Inventory => Player.LivingEntity.Inventory as HumanoidInventory;
    public CinemachineCamera TopDownCamera;

    // coins
    [HideInInspector] public UnityEvent<int> CoinsChangeEvent;
    [SerializeField] private int _coins = 0;
    public int Coins { 
        get{ return _coins; } 
        set {   
            CoinsChangeEvent?.Invoke(value - _coins);
            _coins = value; 
        }  
    }

    public void SetPlayerPosition(Vector3 position) {
        PlayerController.Animator.applyRootMotion = false;
        PlayerController.Animator.speed = 0;
        PlayerController.gameObject.transform.position = position;

        Invoke(nameof(applyRootMotion), 1f);
    }

    private void applyRootMotion() {
        PlayerController.Animator.applyRootMotion = true;
        PlayerController.Animator.speed = 1;
    }


    #endregion
    #region Unity Methods

    void Awake() {
        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion


}
