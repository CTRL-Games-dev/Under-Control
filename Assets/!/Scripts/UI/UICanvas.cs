using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public enum UIState
{
    NotVisible,
    MainMenu,
    PauseScreen,
    Inventory,
    DeathScreen
}


public class UICanvas : MonoBehaviour
{
    public UIState CurrentUIState = UIState.NotVisible;

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
    public bool IsGamePaused = false;


    // UI elements
    private bool _isInventoryOpen = false;
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
    [SerializeField] private GameObject _pauseScreenCanvasGO;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private GameObject _coinsHolder;
    [SerializeField] private GameObject _otherInventoryHolder;
    [SerializeField] private RectTransform _navBarRectTransform;
    private ItemContainer _currentOtherInventory;
    private InventoryCanvas _inventoryCanvas;
    private MainMenu _mainMenu;
    private DeathScreen _deathScreen;
    private PauseScreen _pauseScreen;
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
        
        _inventoryCanvas = _inventoryCanvasGO.GetComponent<InventoryCanvas>();
        _inventoryCanvasGroup = _inventoryCanvasGO.GetComponent<CanvasGroup>();
        _mainMenu = _mainMenuCanvasGO.GetComponent<MainMenu>();
        _deathScreen = _deathScreenCanvasGO.GetComponent<DeathScreen>();
        _pauseScreen = _pauseScreenCanvasGO.GetComponent<PauseScreen>();
    }

    private void Start() {
        EventBus.ItemUIHoverEvent.AddListener(OnItemUIHover);
        EventBus.ItemUIClickEvent.AddListener(OnItemUIClick);
        PlayerController.InventoryToggleEvent.AddListener(OnInventoryToggle);
        PlayerController.UICancelEvent.AddListener(OnUICancel);
        PlayerController.CoinsChangeEvent.AddListener(OnCoinsChange);

        OnCoinsChange(0);
        
        // _inventoryCanvas.SetCurrentTab(InventoryCanvas.InventoryTabs.Armor);
        OpenUIState(CurrentUIState);
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
        _isInventoryOpen = !_isInventoryOpen;
        if (_isInventoryOpen) {
            OpenUIState(UIState.Inventory);
        } else {
            CloseUIState(UIState.Inventory);
        }
    }

    private void OnUICancel() {
        switch (CurrentUIState) {
            case UIState.Inventory:
                CloseUIState(UIState.Inventory);
                break;
            case UIState.PauseScreen:
                CloseUIState(UIState.PauseScreen);
                break;
            case UIState.MainMenu:
                break;
            case UIState.DeathScreen:
                break;
            case UIState.NotVisible:
                OpenUIState(UIState.PauseScreen);
                break;
            default:
                OpenUIState(UIState.PauseScreen);
                break;
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
        else _inventoryCanvas.SetCurrentTab(InventoryCanvas.InventoryTabs.Armor);

        if (_currentOtherInventory == itemContainer) return;

        _lastInteractableInventory?.EndInteract();
        _lastInteractableInventory = interactable;

        _currentOtherInventory = itemContainer;
        _inventoryCanvas.SetOtherTabTitle(title);

        if (itemContainer != null) { // animacja zmiany
            _inventoryCanvas.OtherTabExit().OnComplete(() => {
                if (_otherInventoryHolder.transform.childCount > 0)
                    Destroy(_otherInventoryHolder.transform.GetChild(0).gameObject);
                
                OpenUIState(UIState.Inventory);

                InventoryPanel inventoryPanel = Instantiate(prefab, _otherInventoryHolder.transform).GetComponentInChildren<InventoryPanel>();
                inventoryPanel.SetTargetInventory(itemContainer);
                // inventoryPanel.ConnectSignals();
                _inventoryCanvas.SetCurrentTab(InventoryCanvas.InventoryTabs.Other);
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

    private IEnumerator animateCoins(bool increase) {
        _coinsText.color = increase ? Color.green : Color.red;
        _coinsHolder.transform.localScale = increase ? new Vector3(1.2f, 1f, 1f) : new Vector3(0.8f, 1f, 1f);
        
        yield return new WaitForSeconds(0.25f);
        
        _coinsText.color = Color.white;
        _coinsHolder.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    #endregion
    #region Change UI State

    public void OpenUIState(UIState state) {
        CurrentUIState = state;
        switch (state) {
            case UIState.MainMenu:
                openMainMenu();
                break;
            case UIState.PauseScreen:
                openPauseScreen();
                break;
            case UIState.Inventory:
                openInventoryScreen();
                break;
            case UIState.DeathScreen:
                openDeathScreen();
                break;
            case UIState.NotVisible:
                _HUDCanvasGO.SetActive(true);
                PlayerController.InputDisabled = false;
                break;
        }
    }

    public void CloseUIState(UIState state) {
        CurrentUIState = UIState.NotVisible;
        switch (state) {
            case UIState.MainMenu:
                closeMainMenu();
                break;
            case UIState.PauseScreen:
                closePauseScreen();
                break;
            case UIState.Inventory:
                closeInventoryScreen();
                break;
        }
    }

    private void openMainMenu() {
        PlayerController.InputDisabled = true;
        _mainMenu.OpenMenu();
        _HUDCanvasGO.SetActive(false);
    }

    private void closeMainMenu() {
        PlayerController.InputDisabled = false;
        _mainMenu.CloseMenu();
        _HUDCanvasGO.SetActive(true);
    }

    private void openPauseScreen() {
        IsGamePaused = true;
        _pauseScreen.ShowPauseMenu();
    }

    private void closePauseScreen() {
        IsGamePaused = false;
        _pauseScreen.HidePauseMenu();
    }

    private void openInventoryScreen() {
        InventoryPanel.IsItemJustBought = false;
        _isInventoryOpen = true;

        _inventoryCanvasGroup.DOKill();
        _navBarRectTransform.DOKill();
        _navBarRectTransform.DOAnchorPosY(_isInventoryOpen ? 0 : 65, 0.25f);
        _inventoryCanvasGO.SetActive(true);
        _inventoryCanvasGroup.DOFade(1, 0.25f);
        _inventoryCanvasGroup.interactable = true;
        _inventoryCanvasGroup.blocksRaycasts = true;
        _inventoryCanvas.OpenCurrentTab();
    }

    private void closeInventoryScreen() {
        InventoryPanel.IsItemJustBought = false;
        _isInventoryOpen = false;

        _inventoryCanvasGroup.DOKill();
        _navBarRectTransform.DOKill();
        _navBarRectTransform.DOAnchorPosY(_isInventoryOpen ? 0 : 65, 0.25f);

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

    private void openDeathScreen() {
        PlayerController.InputDisabled = true;
        _deathScreen.ShowDeathScreen();
        _HUDCanvasGO.SetActive(false);
        _alwayOnTopCanvasGO.SetActive(false);
        _inventoryCanvasGO.SetActive(false);
    }


    #endregion
}
