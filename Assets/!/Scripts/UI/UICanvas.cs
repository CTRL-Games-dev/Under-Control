using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
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
    public Camera MainCamera;

    // UI elements
    private bool _isInventoryOpen = false;
    public bool IsInventoryOpen {
        get => _isInventoryOpen;
        set {
            _isInventoryOpen = value;
            openInventory(_isInventoryOpen);
        }
    }
    public InventoryPanel PlayerInventoryPanel;

    [HideInInspector] public InventoryPanel ActiveInventoryPanel;
    public ItemInfoPanel ItemInfoPanel;
    public SelectedItemUI SelectedItemUI;

    // serialized fields
    [Header("Canvases")]
    [SerializeField] private GameObject _HUDCanvasGO;
    [SerializeField] private GameObject _inventoryCanvasGO;
    [SerializeField] private GameObject _alwayOnTopCanvasGO;
    [SerializeField] private GameObject _mainMenuCanvasGO;
    [SerializeField] private GameObject _deathScreenCanvasGO;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private GameObject _coinsHolder;
    [SerializeField] private GameObject _otherInventoryHolder;
    [SerializeField] private RectTransform _navBarRectTransform;
    private ItemContainer _currentOtherInventory;
    private InventoryCanvas _inventoryCanvas;
    private MainMenu _mainMenu;
    private DeathScreen _deathScreen;
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
        _mainMenu = _mainMenuCanvasGO.GetComponent<MainMenu>();
        _deathScreen = _deathScreenCanvasGO.GetComponent<DeathScreen>();
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
        if (SelectedItemUI.InventoryItem != null) return; 

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
        if (IsInventoryOpen) {
            openInventory(false);
        } else {
            OpenMainMenu();
        }
    }

    #endregion

    #region Inventory Methods

    public void SetSelectedItemUI(ItemUI itemUI) {
        if (SelectedItemUI.InventoryItem != null) return;

        SelectedItemUI.gameObject.SetActive(itemUI != null);
        SelectedItemUI.InventoryItem = itemUI.InventoryItem;
    }


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

    public void DropItem() {
        if (SelectedItemUI.InventoryItem == null) return;

        PlayerController.LivingEntity.DropItem(SelectedItemUI.InventoryItem);

        SelectedItemUI.InventoryItem = null;
        EventBus.ItemPlacedEvent?.Invoke();
    }

    private void openInventory(bool value) {
        InventoryPanel.IsItemJustBought = false;
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
                DropItem();
            }

            // _inventoryCanvasGroup.DOFade(0, 0.25f).OnComplete(() => _inventoryCanvasGO.SetActive(false));
            _inventoryCanvasGroup.DOFade(0, 0.25f);
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

    public void OpenMainMenu() {
        PlayerController.InputDisabled = true;
        _mainMenu.OpenMenu();
        _HUDCanvasGO.SetActive(false);
        _alwayOnTopCanvasGO.SetActive(false);
        _inventoryCanvasGO.SetActive(false);
    }

    public void CloseMainMenu() {
        PlayerController.InputDisabled = false;
        _mainMenu.CloseMenu();
        _HUDCanvasGO.SetActive(true);
        _alwayOnTopCanvasGO.SetActive(true);
    }

    public void OpenDeathScreen() {
        PlayerController.InputDisabled = true;
        _deathScreen.ShowDeathScreen();
        _HUDCanvasGO.SetActive(false);
        _alwayOnTopCanvasGO.SetActive(false);
        _inventoryCanvasGO.SetActive(false);
    }


    #endregion
}
