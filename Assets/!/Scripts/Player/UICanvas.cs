using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
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


public class UICanvas : MonoBehaviour
{
    public UIBottomState CurrentUIBottomState = UIBottomState.HUD;
    public UIMiddleState CurrentUIMiddleState = UIMiddleState.NotVisible;
    public UITopState CurrentUITopState = UITopState.NotVisible;

    #region Fields

    [Header("References for children")]
    [HideInInspector] public InventoryPanel ActiveInventoryPanel;
    public ItemInfoPanel ItemInfoPanel;
    public EvoInfo EvoInfo;
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

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            TextData.ChangeLanguage(Language.English);
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            TextData.ChangeLanguage(Language.Polish);
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


    public void SetOtherInventory(ItemContainer itemContainer, GameObject prefab, IInteractableInventory interactable = null, string title = null) {
        InventoryCanvas.SetOtherInventory(itemContainer, prefab, interactable, title);
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
    #region Misc Methods

    public void StartTalking(Dialogue dialogue, Texture faceImage, FaceAnimator faceAnimator, string nameKey) {
        TalkingCanvas.gameObject.SetActive(true);
        if (dialogue == null) return;

        TalkingCanvas.SetupDialogue(dialogue, faceImage, faceAnimator, nameKey);
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
                Player.Instance.InputDisabled = false;
                TalkingCanvas.HideUI();
                break;
        }
    }

    private void openUIBottomState(UIBottomState state) {
        switch (state) {
            case UIBottomState.HUD:
                Player.Instance.InputDisabled = false;
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
                InventoryCanvas.HideUI();                
                break;
            case UIMiddleState.MainMenu:
                Player.Instance.InputDisabled = false;
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
                MainMenuCanvas.ShowUI();
                break;
            case UIMiddleState.Pause:
                PauseCanvas.ShowUI();
                break;
            case UIMiddleState.NotVisible:
                Player.Instance.InputDisabled = false;
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
                _videoPlayer.gameObject.GetComponent<CanvasGroup>().DOFade(1, 1).SetUpdate(true);
                break;
        }
    }


    #endregion
}
