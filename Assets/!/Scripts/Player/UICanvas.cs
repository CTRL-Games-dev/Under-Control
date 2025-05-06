using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;


public enum UIBottomState {
    NotVisible,
    HUD,
    Cutscene,
    Talking
}

public enum UIMiddleState {
    NotVisible,
    Inventory,
    MainMenu,
    Pause,
    Choose
}

public enum UITopState {
    NotVisible,
    Death,
    Settings,
    VideoPlayer
}


public class UICanvas : MonoBehaviour {
    #region Scalling
    
    public static Vector2 ScreenScale { get => new Vector2(Screen.width / 1920f, Screen.height / 1080f); }
    public static Vector2 CanvasScale { get => new Vector2(1920f / Screen.width, 1080f / Screen.height); }

    // Scales a vector to the virtual screen size (1920x1080).
    public static Vector2 ScaleToCanvas(Vector2 original) {
        Vector2 scale = CanvasScale;
        return new Vector2(original.x * scale.x, original.y * scale.y);
    }

    // Scales a vector to the virtual screen size (1920x1080). Z is not scaled.
    public static Vector3 ScaleToCanvas(Vector3 original) {
        Vector2 scale = CanvasScale;
        return new Vector3(original.x * scale.x, original.y * scale.y, original.z);
    }

    public static Vector2 ScaleToScreen(Vector2 original) {
        Vector2 scale = ScreenScale;
        return new Vector2(original.x * scale.x, original.y * scale.y);
    }

    // Scales a vector to the virtual screen size (1920x1080). Z is not scaled.
    public static Vector3 ScaleToScreen(Vector3 original) {
        Vector2 scale = ScreenScale;
        return new Vector3(original.x * scale.x, original.y * scale.y, original.z);
    }

    #endregion

    public UIBottomState CurrentUIBottomState = UIBottomState.HUD;
    public UIMiddleState CurrentUIMiddleState = UIMiddleState.NotVisible;
    public UITopState CurrentUITopState = UITopState.NotVisible;

    #region Fields

    [Header("References for children")]
    [HideInInspector] public InventoryPanel ActiveInventoryPanel;
    public ItemInfoPanel ItemInfoPanel;
    public SelectedItemUI SelectedItemUI;


    [Header("Canvases")]
    // Bottom
    public HUDCanvas HUDCanvas;
    public TalkingCanvas TalkingCanvas;

    // Middle
    public InventoryCanvas InventoryCanvas;
    public MainMenuCanvas MainMenuCanvas;
    public PauseCanvas PauseCanvas;
    public ChooseCanvas ChooseCanvas;

    // Top
    public DeathScreenCanvas DeathScreenCanvas;
    public SettingsCanvas SettingsCanvas;


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
        if (IsOtherUIOpen || CurrentUIBottomState == UIBottomState.Talking) return;
        if (CurrentUIMiddleState == UIMiddleState.NotVisible || CurrentUIMiddleState == UIMiddleState.Inventory) {
            ChangeUIMiddleState(CurrentUIMiddleState == UIMiddleState.Inventory ? UIMiddleState.NotVisible : UIMiddleState.Inventory);
        }
    }

    private void OnUICancel() {
        if (IsOtherUIOpen || CurrentUIMiddleState == UIMiddleState.Choose || CurrentUITopState == UITopState.Death) {
            Debug.Log("Other UI is open");
            return;
        }

        if (CurrentUIBottomState == UIBottomState.Talking) {
            ChangeUIBottomState(UIBottomState.HUD);
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
    #region Misc Methods

    public void StartTalking(Dialogue dialogue, Texture faceImage, FaceAnimator faceAnimator, string nameKey, Talkable talkable) {
        TalkingCanvas.gameObject.SetActive(true);
        if (dialogue == null) return;

        TalkingCanvas.SetupDialogue(dialogue, faceImage, faceAnimator, nameKey, talkable);
        ChangeUIBottomState(UIBottomState.Talking);
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
                HUDCanvas.HideUI();
                break;
            case UIBottomState.Talking:
                TalkingCanvas.HideUI();
                break;
        }
    }

    private void openUIBottomState(UIBottomState state) {
        switch (state) {
            case UIBottomState.HUD:
                HUDCanvas.ShowUI();
                break;
            case UIBottomState.Talking:
                Player.Instance.InputDisabled = true;
                TalkingCanvas.ShowUI();
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
                EventBus.InventoryClosedEvent?.Invoke();
                InventoryCanvas.HideUI();                
                break;
            case UIMiddleState.MainMenu:
                Player.Instance.InputDisabled = false;
                Player.Instance.LockRotation = false;
                Player.Instance.UpdateDisabled = false;
                MainMenuCanvas.HideUI();
                break;
            case UIMiddleState.Pause:
                PauseCanvas.HideUI();
                break;
            case UIMiddleState.Choose:
                ChooseCanvas.HideUI();
                break;
        }
    }

    private void openUIMiddleState(UIMiddleState state) {
        switch (state) {
            case UIMiddleState.Inventory:
                InventoryCanvas.ShowUI();
                break;
            case UIMiddleState.MainMenu:
                closeUIBottomState(CurrentUIBottomState);
                Player.Instance.InputDisabled = true;
                Player.Instance.LockRotation = true;
                MainMenuCanvas.ShowUI();
                break;
            case UIMiddleState.Pause:
                PauseCanvas.ShowUI();
                break;
            case UIMiddleState.NotVisible:
                Player.Instance.InputDisabled = false;
                Player.Instance.LockRotation = false;
                InventoryPanel.IsItemJustBought = false;
                DropItem();
                InventoryCanvas.SetOtherInventory(null, null);
                break;
            case UIMiddleState.Choose:
                ChooseCanvas.ShowUI();
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
                DeathScreenCanvas.HideUI();
                break;
            case UITopState.Settings:
                SettingsCanvas.HideUI();
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
                DeathScreenCanvas.ShowUI();
                break;
            case UITopState.Settings:
                SettingsCanvas.ShowUI();
                break;
            case UITopState.VideoPlayer:
                Debug.Log("VideoPlayer");

                _videoPlayer.gameObject.SetActive(true);
                _videoPlayer.Play();
                _videoPlayer.gameObject.GetComponent<CanvasGroup>().DOFade(1, 1 * Settings.AnimationSpeed).SetUpdate(true);
                break;
        }
    }


    #endregion
}
