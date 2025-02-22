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
    public EntityInventory PlayerInventory { get => PlayerLivingEntity.Inventory; }

    public ItemInfoPanel ItemInfoPanel;
    public SelectedItemUI SelectedItemUI;
    public InventoryPanel ActiveInventoryPanel;
    private bool _isInventoryOpen = false;
    public bool IsInventoryOpen {
        get => _isInventoryOpen;
        set {
            _isInventoryOpen = value;
            _inventoryBGCanvasGroup.DOKill();
            if (_isInventoryOpen) {
                InventoryBG.SetActive(true);
                _inventoryBGCanvasGroup.DOFade(1, 0.25f);
                _inventoryBGCanvasGroup.interactable = true;
                _inventoryBGCanvasGroup.blocksRaycasts = true;
            } else {
                _inventoryBGCanvasGroup.DOFade(0, 0.25f).OnComplete(() => InventoryBG.SetActive(false));
                _inventoryBGCanvasGroup.interactable = false;
                _inventoryBGCanvasGroup.blocksRaycasts = false;
            }
        }
    }

    public GameObject InventoryBG;
    private CanvasGroup _inventoryBGCanvasGroup;

    // serialized fields
    [SerializeField] private GameObject _AlwayOnTopCanvas;
    [SerializeField] private GameObject _;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private GameObject _coinsHolder;

    private void Start() {
        EventBus.ItemUIHoverEvent.AddListener(OnItemUIHover);
        EventBus.ItemUIClickEvent.AddListener(OnItemUIClick);
        PlayerController.InventoryToggleEvent.AddListener(OnInventoryToggle);
        PlayerController.UICancelEvent.AddListener(OnUICancel);

        PlayerController.CoinsChangeEvent.AddListener(OnCoinsChange);

        _inventoryBGCanvasGroup = InventoryBG.GetComponent<CanvasGroup>();

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
        _coinsText.text = "×" + (PlayerController.Coins + change).ToString();
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
