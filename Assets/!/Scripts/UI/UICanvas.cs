using System.Collections;
using DG.Tweening;
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

    // UI elements
    private bool _isInventoryOpen = false;
    public bool IsInventoryOpen {
        get => _isInventoryOpen;
        set {
            _isInventoryOpen = value;
            _inventoryBGCanvasGroup.DOKill();
            if (_isInventoryOpen) {
                _inventoryCanvas.SetActive(true);
                _inventoryBGCanvasGroup.DOFade(1, 0.25f);
                _inventoryBGCanvasGroup.interactable = true;
                _inventoryBGCanvasGroup.blocksRaycasts = true;
            } else {
                _inventoryBGCanvasGroup.DOFade(0, 0.25f).OnComplete(() => _inventoryCanvas.SetActive(false));
                _inventoryBGCanvasGroup.interactable = false;
                _inventoryBGCanvasGroup.blocksRaycasts = false;
            }
        }
    }

    [HideInInspector] public InventoryPanel ActiveInventoryPanel;

    public ItemInfoPanel ItemInfoPanel;
    public SelectedItemUI SelectedItemUI;

    // serialized fields
    [Header("Canvases")]
    [SerializeField] private GameObject _HUDCanvas;
    [SerializeField] private GameObject _inventoryCanvas;
    [SerializeField] private GameObject _alwayOnTopCanvas;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private GameObject _coinsHolder;
    private CanvasGroup _inventoryBGCanvasGroup;

    private void Start() {
        EventBus.ItemUIHoverEvent.AddListener(OnItemUIHover);
        EventBus.ItemUIClickEvent.AddListener(OnItemUIClick);
        PlayerController.InventoryToggleEvent.AddListener(OnInventoryToggle);
        PlayerController.UICancelEvent.AddListener(OnUICancel);

        PlayerController.CoinsChangeEvent.AddListener(OnCoinsChange);

        _inventoryBGCanvasGroup = _inventoryCanvas.GetComponent<CanvasGroup>();

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
    }

    private void OnUICancel() {
        if (IsInventoryOpen) {
            IsInventoryOpen = false;
        }
    }


    private IEnumerator animateCoins(bool increase) {
        _coinsText.color = increase ? Color.green : Color.red;
        _coinsHolder.transform.localScale = increase ? new Vector3(1.2f, 1f, 1f) : new Vector3(0.8f, 1f, 1f);
        
        yield return new WaitForSeconds(0.25f);
        
        _coinsText.color = Color.white;
        _coinsHolder.transform.localScale = new Vector3(1f, 1f, 1f);
    }

}
