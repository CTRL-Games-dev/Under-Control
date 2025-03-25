using Unity.Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Singleton
    public static Player Instance;
    public static UICanvas UICanvas => Player.Instance._uiCanvas;
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
    public static CinemachineCamera TopDownCamera => Player.Instance._topDownCamera;


    [SerializeField] private UICanvas _uiCanvas;
    [SerializeField] private CinemachineCamera _topDownCamera;

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
