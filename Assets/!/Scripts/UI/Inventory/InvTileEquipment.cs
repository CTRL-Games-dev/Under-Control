using UnityEngine;

public class InvTileEquipment : InvTile {
    private enum TileType {
        Material,
        Helmet,
        Chestplate,
        Leggings,
        Boots,
        Amulet,
        Ring,
        WeaponLeftHand,
        WeaponRightHand,
    }

    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private TileType _tileType;

    private RectTransform _rectTransform;
    private UICanvas _uiCanvas;
    private ItemUI _itemUI;
    
    private InventoryItem SelectedInventoryItem => _uiCanvas.SelectedItemUI.InventoryItem;

    private void Start() {
        EventBus.TileSizeSetEvent.AddListener(OnTileSizeSetEvent);
        EventBus.ItemUIClickEvent.AddListener(OnItemUIClickEvent);

        _uiCanvas = UICanvas.Instance;
        _rectTransform = GetComponent<RectTransform>();
        // EventBus.InvTileClickEvent.AddListener(OnTileClick);
        OnTileSizeSetEvent();
    }

    private void OnTileSizeSetEvent() {
        _rectTransform.sizeDelta = new Vector2(InventoryPanel.TileSize, InventoryPanel.TileSize);
    }

    private void OnItemUIClickEvent(ItemUI itemUI) {
        if (itemUI == null || SelectedInventoryItem != null) return;
        if (itemUI == _itemUI) {
            switch (_tileType) {
                case TileType.Helmet:
                    _uiCanvas.PlayerInventory.Helmet = null;
                    break;
                case TileType.Chestplate:
                    _uiCanvas.PlayerInventory.Chestplate = null;
                    break;
                case TileType.Leggings:
                    _uiCanvas.PlayerInventory.Leggings = null;
                    break;
                case TileType.Boots:
                    _uiCanvas.PlayerInventory.Boots = null;
                    break;
                case TileType.Amulet:
                    _uiCanvas.PlayerInventory.Amulet = null;
                    break;
                case TileType.Ring:
                    _uiCanvas.PlayerInventory.Ring = null;
                    break;
                case TileType.WeaponLeftHand:
                    _uiCanvas.PlayerInventory.UnequipLeftHand();
                    break;
                case TileType.WeaponRightHand:
                    _uiCanvas.PlayerInventory.UnequipRightHand();
                    break;
            }

            IsEmpty = true;
            Destroy(_itemUI.gameObject);
            _itemUI = null;
        }
    }

    public void OnPointerClickEquipment() {
        if (SelectedInventoryItem == null && _itemUI == null) return;        
        if (SelectedInventoryItem != null && _itemUI != null) return;
        if(!IsEmpty) return;

        if (_tileType == TileType.Helmet) {
            if (SelectedInventoryItem.ItemData is not HelmetItemData helmetItemData) {
                return;
            }

            _uiCanvas.PlayerInventory.Helmet = helmetItemData;
        } else if (_tileType == TileType.Chestplate) {
            if (SelectedInventoryItem.ItemData is not ChestplateItemData chestplateItemData) {
                return;
            }

            _uiCanvas.PlayerInventory.Chestplate = chestplateItemData;
        } else if (_tileType == TileType.Leggings) {
            if (SelectedInventoryItem.ItemData is not LeggingsItemData leggingsItemData) {
                return;
            }

            _uiCanvas.PlayerInventory.Leggings = leggingsItemData;
        } else if (_tileType == TileType.Boots) {
            if (SelectedInventoryItem.ItemData is not BootsItemData bootsItemData) {
                return;
            }

            _uiCanvas.PlayerInventory.Boots = bootsItemData;
        } else if (_tileType == TileType.Ring) {
            if (SelectedInventoryItem.ItemData is not RingItemData ringItemData) {
                return;
            }

            _uiCanvas.PlayerInventory.Ring = ringItemData;
        } else if (_tileType == TileType.Amulet) {
            if (SelectedInventoryItem.ItemData is not AmuletItemData amuletItemData) {
                return;
            }

            _uiCanvas.PlayerInventory.Amulet = amuletItemData;
        } else if (_tileType == TileType.WeaponLeftHand) {
            if (SelectedInventoryItem.ItemData is not WeaponItemData weaponItemData) {
                return;
            }

            _uiCanvas.PlayerInventory.EquipLeftHand(weaponItemData);
        } else if (_tileType == TileType.WeaponRightHand) {
            if (SelectedInventoryItem.ItemData is not WeaponItemData weaponItemData) {
                return;
            }

            _uiCanvas.PlayerInventory.EquipRightHand(weaponItemData);
        }

        createItemUI(SelectedInventoryItem);
        EventBus.ItemPlacedEvent?.Invoke();
        IsEmpty = false;
        _uiCanvas.SelectedItemUI.InventoryItem = null;
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
