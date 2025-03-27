using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;


public enum UIBottomState {
    NotVisible,
    HUD,
    Cutscene
}

public enum UIMiddleState {
    NotVisible,
    Inventory,
    MainMenu,
    Pause,
}

public enum UITopState {
    NotVisible,
    Death,
    Settings,
    VideoPlayer
}


public class UICanvas : MonoBehaviour
{
    public UIBottomState CurrentUIBottomState = UIBottomState.HUD;
    public UIMiddleState CurrentUIMiddleState = UIMiddleState.NotVisible;
    public UITopState CurrentUITopState = UITopState.NotVisible;

    #region Fields

    [Header("References for children")]
    public Camera MainCamera;
    public bool IsGamePaused = false;

    // UI elements
    public InventoryPanel PlayerInventoryPanel;

    [HideInInspector] public InventoryPanel ActiveInventoryPanel;
    public ItemInfoPanel ItemInfoPanel;
    public SelectedItemUI SelectedItemUI;


    [Header("Canvases")]
    [SerializeField] private HUDCanvas _HUDCanvas;
    [SerializeField] private InventoryCanvas _inventoryCanvas;
    [SerializeField] private MainMenuCanvas _mainMenuCanvas;
    [SerializeField] private DeathScreenCanvas _deathScreenCanvas;
    [SerializeField] private PauseCanvas _pauseCanvas;
    [SerializeField] private SettingsCanvas _settingsCanvas;


    [Header("UI Elements")]
    [SerializeField] private RectTransform _navBarRectTransform;
    [SerializeField] private ActionNotifierManager _actionNotifierManager;

    public bool IsOtherUIOpen = false;

    [SerializeField] private VideoPlayer _videoPlayer;

    #endregion

    #region Unity Methods

    private void Start() {
        EventBus.ItemUIHoverEvent.AddListener(OnItemUIHover);
        Player.Instance.InventoryToggleEvent.AddListener(OnInventoryToggle);
        Player.Instance.UICancelEvent.AddListener(OnUICancel);
        
        if (!LoadingScreen.IsLoading) {
            ChangeUIBottomState(CurrentUIBottomState);
            ChangeUIMiddleState(CurrentUIMiddleState);
            ChangeUITopState(CurrentUITopState);
        }
    }

    #endregion

    #region Callbacks

    private void OnItemUIHover(ItemUI itemUI) {
        if (SelectedItemUI.InventoryItem != null) return;
        ItemInfoPanel.ShowItemInfo(itemUI);
    }    

    private void OnInventoryToggle() {
        if (IsOtherUIOpen) return;
        ChangeUIMiddleState(CurrentUIMiddleState == UIMiddleState.Inventory ? UIMiddleState.NotVisible : UIMiddleState.Inventory);
    }

    private void OnUICancel() {
        if (IsOtherUIOpen) {
            Debug.Log("Other UI is open");
            return;
        }

        if (CurrentUITopState != UITopState.NotVisible) {
            ChangeUITopState(UITopState.NotVisible);
            return;
        } 
        if (CurrentUIMiddleState == UIMiddleState.NotVisible) {
            ChangeUIMiddleState(UIMiddleState.Pause);
        } else {
            ChangeUIMiddleState(UIMiddleState.NotVisible);
        }
    }

    #endregion

    #region Inventory Methods

    public void SetSelectedItemUI(ItemUI itemUI) {
        if (SelectedItemUI.InventoryItem != null) return;

        SelectedItemUI.gameObject.SetActive(itemUI != null);
        SelectedItemUI.InventoryItem = itemUI.InventoryItem;
    }

    public void SetSelectedInventoryItem(InventoryItem inventoryItem) {
        if (SelectedItemUI.InventoryItem != null) return;

        SelectedItemUI.gameObject.SetActive(inventoryItem != null);
        SelectedItemUI.InventoryItem = inventoryItem;
    }


    public void SetOtherInventory(ItemContainer itemContainer, GameObject prefab, IInteractableInventory interactable = null, string title = null) {
        _inventoryCanvas.SetOtherInventory(itemContainer, prefab, interactable, title);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.Inventory);
    }

    public void DropItem() {
        if (SelectedItemUI.InventoryItem == null) return;

        Player.LivingEntity.DropItem(SelectedItemUI.InventoryItem);

        SelectedItemUI.InventoryItem = null;
        EventBus.ItemPlacedEvent?.Invoke();
    }

    public void PickupItemNotify(ItemData itemData, int amount) {
        _actionNotifierManager.SpawnActionNotifier(itemData.Icon, itemData.DisplayName, Color.white, amount);
    }

    #endregion
    #region Change UI State

    // Bottom 
    public void ChangeUIBottomState(UIBottomState state) {
        closeUIBottomState(CurrentUIBottomState);
        openUIBottomState(state);
        CurrentUIBottomState = state;
    }

    private void closeUIBottomState(UIBottomState state) {
        switch (state) {
            case UIBottomState.HUD:
                _HUDCanvas.HideUI();
                break;
        }
    }

    private void openUIBottomState(UIBottomState state) {
        switch (state) {
            case UIBottomState.HUD:
                _HUDCanvas.ShowUI();
                break;
        }
    }

    // Middle
    public void ChangeUIMiddleState(UIMiddleState state) {
        closeUIMiddleState(CurrentUIMiddleState);
        openUIMiddleState(state);
        CurrentUIMiddleState = state;
    }

    private void closeUIMiddleState(UIMiddleState state) {
        switch (state) {
            case UIMiddleState.Inventory:
                _inventoryCanvas.HideUI();                
                break;
            case UIMiddleState.MainMenu:
                Player.Instance.InputDisabled = false;
                _mainMenuCanvas.HideUI();
                break;
            case UIMiddleState.Pause:
                _pauseCanvas.HideUI();
                break;
        }
    }

    private void openUIMiddleState(UIMiddleState state) {
        switch (state) {
            case UIMiddleState.Inventory:
                _inventoryCanvas.ShowUI();
                break;
            case UIMiddleState.MainMenu:
                closeUIBottomState(CurrentUIBottomState);
                Player.Instance.InputDisabled = true;
                _mainMenuCanvas.ShowUI();
                break;
            case UIMiddleState.Pause:
                _pauseCanvas.ShowUI();
                break;
            case UIMiddleState.NotVisible:
                Player.Instance.InputDisabled = false;
                InventoryPanel.IsItemJustBought = false;
                DropItem();
                _inventoryCanvas.SetOtherInventory(null, null);
                break;
        }
    }

    // Top
    public void ChangeUITopState(UITopState state) {
        closeUITopState(CurrentUITopState);
        openUITopState(state);
        CurrentUITopState = state;
    }

    private void closeUITopState(UITopState state) {
        switch (state) {
            case UITopState.Death:
                _deathScreenCanvas.HideUI();
                break;
            case UITopState.Settings:
                _settingsCanvas.HideUI();
                break;
            case UITopState.VideoPlayer:
                _videoPlayer.Stop();
                _videoPlayer.gameObject.SetActive(false);
                break;
        }
    }

    private void openUITopState(UITopState state) {
        switch (state) {
            case UITopState.Death:
                Player.Instance.InputDisabled = true;
                _deathScreenCanvas.ShowUI();
                break;
            case UITopState.Settings:
                _settingsCanvas.ShowUI();
                break;
            case UITopState.VideoPlayer:
                _videoPlayer.gameObject.SetActive(true);
                _videoPlayer.gameObject.GetComponent<CanvasGroup>().DOFade(1, 1).SetUpdate(true).OnComplete(() => {
                    _videoPlayer.Play();
                });
                break;
        }
    }


    #endregion
}
