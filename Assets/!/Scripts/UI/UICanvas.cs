using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    #region Fields

    [Header("References for children")]
    public static UICanvas Instance;
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
            openInventory(_isInventoryOpen);
        }
    }

    [HideInInspector] public InventoryPanel ActiveInventoryPanel;
    public ItemInfoPanel ItemInfoPanel;
    public SelectedItemUI SelectedItemUI;

    // serialized fields
    [Header("Canvases")]
    [SerializeField] private GameObject _HUDCanvas;
    [SerializeField] private GameObject _inventoryCanvasGO;
    [SerializeField] private GameObject _alwayOnTopCanvasGO;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private GameObject _coinsHolder;
    [SerializeField] private GameObject _otherInventoryHolder;
    [SerializeField] private RectTransform _navBarRectTransform;
    private ItemContainer _currentOtherInventory;
    private InventoryCanvas _inventoryCanvas;
    private CanvasGroup _inventoryCanvasGroup;
    private IInteractableInventory _lastInteractableInventory;

    #endregion

    #region Unity Methods

    private void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start() {
        EventBus.ItemUIHoverEvent.AddListener(OnItemUIHover);
        EventBus.ItemUIClickEvent.AddListener(OnItemUIClick);
        PlayerController.InventoryToggleEvent.AddListener(OnInventoryToggle);
        PlayerController.UICancelEvent.AddListener(OnUICancel);

        PlayerController.CoinsChangeEvent.AddListener(OnCoinsChange);

        _inventoryCanvas = _inventoryCanvasGO.GetComponent<InventoryCanvas>();
        _inventoryCanvasGroup = _inventoryCanvasGO.GetComponent<CanvasGroup>();
        OnCoinsChange(0);
        
        _inventoryCanvas.SetCurrentTab(InventoryCanvas.InventoryTabs.Armor);
        openInventory(false);
    }

    #endregion

    #region Callbacks

    private void OnItemUIHover(ItemUI itemUI) {
        if (SelectedItemUI.InventoryItem != null) return;
        ItemInfoPanel.ShowItemInfo(itemUI);
    }

    private void OnItemUIClick(ItemUI itemUI) {
        SelectedItemUI.gameObject.SetActive(itemUI != null);
        SelectedItemUI.InventoryItem = itemUI.InventoryItem;
    }

    private void OnCoinsChange(int change) {
        _coinsText.text = $"{PlayerController.Coins + change}";
        StartCoroutine(animateCoins(change > 0));
    }

    private void OnInventoryToggle() {
        openInventory(!IsInventoryOpen);
    }

    private void OnUICancel() {
        openInventory(false);
    }

    #endregion

    #region Inventory Methods

    public void SetOtherInventory(ItemContainer itemContainer, GameObject prefab, IInteractableInventory interactable = null, string title = null) {
        if (itemContainer != null) _inventoryCanvas.SetCurrentTab(InventoryCanvas.InventoryTabs.Other);

        if (_currentOtherInventory == itemContainer) return;

        _lastInteractableInventory?.EndInteract();
        _lastInteractableInventory = interactable;

        _currentOtherInventory = itemContainer;
        _inventoryCanvas.SetOtherTabTitle(title);

        if (itemContainer != null) { // animacja zmiany
            _inventoryCanvas.OtherTabExit().OnComplete(() => {
                if (_otherInventoryHolder.transform.childCount > 0)
                    Destroy(_otherInventoryHolder.transform.GetChild(0).gameObject);
                
                openInventory(true);

                InventoryPanel inventoryPanel = Instantiate(prefab, _otherInventoryHolder.transform).GetComponentInChildren<InventoryPanel>();
                inventoryPanel.SetTargetInventory(itemContainer);
                // inventoryPanel.ConnectSignals();
                _inventoryCanvas.OtherTabEnter();
            });
        } else {
            if (_otherInventoryHolder.transform.childCount > 0)
                Destroy(_otherInventoryHolder.transform.GetChild(0).gameObject);
        }
    }

    public void DropItem(Vector3 position) {
        if (SelectedItemUI.InventoryItem == null) return;
        ItemEntity.Spawn(SelectedItemUI.InventoryItem.ItemData, SelectedItemUI.InventoryItem.Amount, position);
        SelectedItemUI.InventoryItem = null;
        EventBus.ItemPlacedEvent?.Invoke();
    }

    public void DropItem() {
        DropItem(Player.transform.position + Player.transform.forward * 2);
    }

    private void openInventory(bool value) {
        _isInventoryOpen = value;

        _inventoryCanvasGroup.DOKill();
        _navBarRectTransform.DOKill();
        _navBarRectTransform.DOAnchorPosY(_isInventoryOpen ? 0 : 65, 0.25f);
        if (_isInventoryOpen) {
            _inventoryCanvasGO.SetActive(true);
            _inventoryCanvasGroup.DOFade(1, 0.25f);
            _inventoryCanvasGroup.interactable = true;
            _inventoryCanvasGroup.blocksRaycasts = true;
            _inventoryCanvas.OpenCurrentTab();
        } else {
            if (SelectedItemUI.InventoryItem != null) {
                DropItem(Player.transform.position + Player.transform.forward * 2);
            }

            _inventoryCanvasGroup.DOFade(0, 0.25f).OnComplete(() => _inventoryCanvasGO.SetActive(false));
            _inventoryCanvasGroup.interactable = false;
            _inventoryCanvasGroup.blocksRaycasts = false;
            SetOtherInventory(null, null);
            _inventoryCanvas.CloseAllTabs();
        }
    }

    private IEnumerator animateCoins(bool increase) {
        _coinsText.color = increase ? Color.green : Color.red;
        _coinsHolder.transform.localScale = increase ? new Vector3(1.2f, 1f, 1f) : new Vector3(0.8f, 1f, 1f);
        
        yield return new WaitForSeconds(0.25f);
        
        _coinsText.color = Color.white;
        _coinsHolder.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    #endregion
}
