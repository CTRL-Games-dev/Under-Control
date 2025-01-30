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

    public ItemInfoPanel ItemInfoPanel;
    public SelectedItemUI SelectedItemUI;
    public InventoryPanel ActiveInventoryPanel;


    private void Start() {
        EventBus.ItemUIHoverEvent.AddListener(OnItemUIHover);
        EventBus.ItemUIClickEvent.AddListener(OnItemUIClick);
    }

    private void OnItemUIHover(InventoryItem item) {
        if (SelectedItemUI.InventoryItem != null) return;
        ItemInfoPanel.ShowItemInfo(item);
    }

    private void OnItemUIClick(ItemUI itemUI) {
        SelectedItemUI.gameObject.SetActive(itemUI != null);
        SelectedItemUI.InventoryItem = itemUI.InventoryItem;
    }

}
