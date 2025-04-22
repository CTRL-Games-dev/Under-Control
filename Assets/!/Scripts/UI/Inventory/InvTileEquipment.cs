using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InvTileEquipment : InvTile {
    private enum TileType {
        Armor,
        Amulet,
        Weapon,
        Consumeable1,
        Consumeable2,
    }

    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private TileType _tileType;
    [SerializeField] private GameObject _hintImage;
    [SerializeField] private Image _bgImage, _highlightImg;
    [SerializeField] private Sprite _singleBg, _doubleBg, _tripleBg;
    [SerializeField] private Sprite _singleHighlight, _doubleHighlight, _tripleHighlight;
    private RectTransform _rectTransform;
    private ItemUI _itemUI;
    
    private InventoryItem SelectedInventoryItem => Player.UICanvas.SelectedItemUI.InventoryItem;

    private void Start() {
        EventBus.TileSizeSetEvent.AddListener(OnTileSizeSetEvent);
        EventBus.ItemUILeftClickEvent.AddListener(OnItemUIClickEvent);
        EventBus.ItemUIRightClickEvent.AddListener(OnItemUIClickEvent);

        _rectTransform = GetComponent<RectTransform>();
        // EventBus.InvTileClickEvent.AddListener(OnTileClick);
        OnTileSizeSetEvent();

        switch (_tileType) {
            case TileType.Armor:
                if (Player.Inventory.Armor != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem = Player.Inventory.Armor;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;

            case TileType.Amulet:
                if (Player.Inventory.Amulet != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem = Player.Inventory.Amulet;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;

            case TileType.Weapon:
                if (Player.Inventory.Weapon != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem = Player.Inventory.Weapon;
                    inventoryItem.Rotated = false;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break; 
            
            case TileType.Consumeable1:
                Player.Instance.UpdateConsumablesEvent.AddListener(OnConsumablesUpdate);
                if (Player.Instance.ConsumableItemOne.ItemData != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Instance.ConsumableItemOne.ItemData;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
            
            case TileType.Consumeable2:
                Player.Instance.UpdateConsumablesEvent.AddListener(OnConsumablesUpdate);
                if (Player.Instance.ConsumableItemTwo.ItemData != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Instance.ConsumableItemTwo.ItemData;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
        }
    }

    private void OnTileSizeSetEvent() {
        _rectTransform.sizeDelta = new Vector2(InventoryPanel.TileSize, InventoryPanel.TileSize * (_tileType == TileType.Weapon ? 3 : 1));
    }

    public void UpdateInvTile() {
        if (_itemUI.InventoryItem.Amount <= 0) {
            Destroy(_itemUI.gameObject);
            _itemUI = null;
            IsEmpty = true;
        } 
    }

    private void OnItemUIClickEvent(ItemUI itemUI) {
        // Debug.Log("ItemUIClickEvent");
        // if (itemUI == _itemUI) 
        //     Destroy(_itemUI.gameObject);
        if (itemUI == null || Player.UICanvas.SelectedItemUI.InventoryItem != null) return;
        
        if (itemUI == _itemUI) {
            switch (_tileType) {
                case TileType.Armor:
                    Player.Inventory.Armor = null;
                    break;
                case TileType.Amulet:
                    Player.Inventory.Amulet = null;
                    break;
                case TileType.Weapon:
                    Player.Inventory.Weapon = null;
                    break;
                case TileType.Consumeable1:
                    Player.Instance.ConsumableItemOne = null;
                    Player.UICanvas.HUDCanvas.OnUpdateConsumables();
                    break;
                case TileType.Consumeable2:
                    Player.Instance.ConsumableItemTwo = null;
                    Player.UICanvas.HUDCanvas.OnUpdateConsumables();
                    break;
            }

            IsEmpty = true;
            _hintImage.SetActive(true);

            if (_tileType == TileType.Weapon) {
                _rectTransform.DOKill();
                _rectTransform.DOSizeDelta(new Vector2(InventoryPanel.TileSize, InventoryPanel.TileSize * 3), 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutBack).OnComplete(() => {
                    _bgImage.sprite = _tripleBg;
                    _highlightImg.sprite = _tripleHighlight;
                });
            }

            if (_itemUI != null) {
                Player.UICanvas.SetSelectedItemUI(_itemUI);
                Destroy(_itemUI.gameObject);
                _itemUI = null;
            }
        }
    }

    public void OnPointerClickEquipment() {
        if (SelectedInventoryItem == null && _itemUI == null) return;        
        if (SelectedInventoryItem != null && _itemUI != null) return;
        if(!IsEmpty) return;
            if (_tileType == TileType.Armor) {
                if(!SelectedInventoryItem.TryAs(out InventoryItem<ArmorItemData> armorItem)) {
                    return;
                }

                Player.Inventory.Armor = armorItem;
            } else if (_tileType == TileType.Amulet) {
                if(!SelectedInventoryItem.TryAs(out InventoryItem<AmuletItemData> amuletItem)) {
                    return;
                }

                Player.Inventory.Amulet = amuletItem;
            } else if (_tileType == TileType.Weapon) {
                if(!SelectedInventoryItem.TryAs(out InventoryItem<WeaponItemData> weaponItem)) {
                    return;
                }

                Player.Inventory.Weapon = weaponItem;
            } else if (_tileType == TileType.Consumeable1) {
                if(!SelectedInventoryItem.TryAs(out InventoryItem<ConsumableItemData> consumableItem)) {
                    return;
                }

                Player.Instance.ConsumableItemOne = consumableItem;
                Player.UICanvas.HUDCanvas.OnUpdateConsumables();
            } else if (_tileType == TileType.Consumeable2) {
                if(!SelectedInventoryItem.TryAs(out InventoryItem<ConsumableItemData> consumableItem)) {
                    return;
                }

                Player.Instance.ConsumableItemTwo = consumableItem;
                Player.UICanvas.HUDCanvas.OnUpdateConsumables();
            }


        _hintImage.SetActive(false);
        
        InventoryItem item = SelectedInventoryItem;
        item.Rotated = _tileType == TileType.Weapon ? false : SelectedInventoryItem.Rotated;

        if (_tileType == TileType.Weapon) {

            Sprite goalSprite;

            if (item.Size.y == 1) {
                goalSprite = _singleBg;
                _highlightImg.sprite = _singleHighlight;
            } else if (item.Size.y == 2) {
                goalSprite = _doubleBg;
                _highlightImg.sprite = _doubleHighlight;
            } else {
                goalSprite = _tripleBg;
                _highlightImg.sprite = _tripleHighlight;
            }

            
            _rectTransform.DOKill();
            _rectTransform.DOSizeDelta(new Vector2(InventoryPanel.TileSize, InventoryPanel.TileSize * item.Size.y), 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutBack).OnComplete(() => {
                _bgImage.sprite = goalSprite;
            });
        }

        createItemUI(SelectedInventoryItem);

        

        EventBus.ItemPlacedEvent?.Invoke();
        IsEmpty = false;
        Player.UICanvas.SelectedItemUI.InventoryItem = null;
    }

    private void OnConsumablesUpdate() {
        InventoryItem consumable = _tileType == TileType.Consumeable1 ? Player.Instance.ConsumableItemOne : Player.Instance.ConsumableItemTwo;

        if (consumable.ItemData == null) {
            if (_itemUI != null) {
                Destroy(_itemUI.gameObject);
                _itemUI = null;
                IsEmpty = true;
            }
        }  else {
            _itemUI.InventoryItem = consumable;
            _itemUI.UpdateAmount();
        }
    }

    private GameObject createItemUI(InventoryItem inventoryItem){
        GameObject itemGameObject = Instantiate(_itemPrefab, _rectTransform);
        itemGameObject.name = inventoryItem.ItemData.DisplayName;

        inventoryItem.Position = new Vector2Int(0, 0);
        inventoryItem.RectTransform = itemGameObject.GetComponent<RectTransform>();
        inventoryItem.ItemUI = itemGameObject.GetComponent<ItemUI>();
        inventoryItem.ItemUI.SetupItem(inventoryItem, InventoryPanel.TileSize, null, null);

        _itemUI = inventoryItem.ItemUI;
        return itemGameObject;
    }
}
