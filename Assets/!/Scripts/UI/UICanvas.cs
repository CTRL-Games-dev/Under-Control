using System.Collections;
using TMPro;
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
    public HumanoidInventory PlayerInventory { get => PlayerLivingEntity.Inventory as HumanoidInventory; }

    public ItemInfoPanel ItemInfoPanel;
    public SelectedItemUI SelectedItemUI;
    public InventoryPanel ActiveInventoryPanel;
    public bool IsInventoryOpen = false;
    public GameObject InventoryBG;

    // serialized fields
    [SerializeField] private GameObject _AlwayOnTopCanvas;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private GameObject _coinsHolder;

    private void Start() {
        EventBus.ItemUIHoverEvent.AddListener(OnItemUIHover);
        EventBus.ItemUIClickEvent.AddListener(OnItemUIClick);
        PlayerController.CoinsChangeEvent.AddListener(OnCoinsChange);
        PlayerController.InventoryToggleEvent.AddListener(OnInventoryToggle);

        OnCoinsChange(0);
    }

    private void OnItemUIHover(ItemUI itemUI) {
        if (SelectedItemUI.InventoryItem != null) return;
        ItemInfoPanel.ShowItemInfo(itemUI);
    }

    private void OnItemUIClick(ItemUI itemUI) {
        SelectedItemUI.gameObject.SetActive(itemUI != null);
        SelectedItemUI.InventoryItem = itemUI.InventoryItem;
    }

    private void OnCoinsChange(int change) {
        _coinsText.text = $"x{PlayerController.Coins + change}";
        StartCoroutine(animateCoins(change > 0));
    }

    private void OnInventoryToggle() {
        IsInventoryOpen = !IsInventoryOpen;
        // InventoryBG.SetActive(IsInventoryOpen);
        // _AlwayOnTopCanvas.SetActive(IsInventoryOpen);
    }


    private IEnumerator animateCoins(bool increase) {
        _coinsText.color = increase ? Color.green : Color.red;
        _coinsHolder.transform.localScale = increase ? new Vector3(1.2f, 1f, 1f) : new Vector3(0.8f, 1f, 1f);
        
        yield return new WaitForSeconds(0.25f);
        
        _coinsText.color = Color.white;
        _coinsHolder.transform.localScale = new Vector3(1f, 1f, 1f);
    }

}
