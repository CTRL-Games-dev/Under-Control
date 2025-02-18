using Unity.VisualScripting;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [Header("References for children")]
    public GameObject Player;

    private PlayerController _playerController;
    public PlayerController PlayerController {
        get {
            if (_playerController == null) {
                _playerController = Player.GetComponent<PlayerController>();
            }
            
            return _playerController;
        }
    }

    private LivingEntity _livingEntity;
    public LivingEntity PlayerLivingEntity {
        get {
            if (_livingEntity == null) {
                _livingEntity = Player.GetComponent<LivingEntity>();
            }

            return _livingEntity;
        }
    }

    public EntityInventory PlayerInventory { get => PlayerLivingEntity.Inventory; }
}
