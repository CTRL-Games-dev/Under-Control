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
        Weapon,
    }

    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private TileType _tileType;

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

        // cant really test this without the rest of the game 
        switch (_tileType) {
            case TileType.Helmet:
                if (Player.Inventory.Helmet != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Inventory.Helmet;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
            case TileType.Chestplate:
                if (Player.Inventory.Chestplate != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Inventory.Chestplate;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
            case TileType.Leggings:
                if (Player.Inventory.Leggings != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Inventory.Leggings;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
            case TileType.Boots:
                if (Player.Inventory.Boots != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Inventory.Boots;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
            case TileType.Amulet:
                if (Player.Inventory.Amulet != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Inventory.Amulet;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
            case TileType.Ring:
                if (Player.Inventory.Ring != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Inventory.Ring;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
            case TileType.Weapon:
                if (Player.Inventory.Weapon != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Inventory.Weapon;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break; 
        }
    }

    private void OnTileSizeSetEvent() {
        _rectTransform.sizeDelta = new Vector2(InventoryPanel.TileSize, InventoryPanel.TileSize);
    }

    private void OnItemUIClickEvent(ItemUI itemUI) {
        // Debug.Log("ItemUIClickEvent");
        // if (itemUI == _itemUI) 
        //     Destroy(_itemUI.gameObject);
        if (itemUI == null || Player.UICanvas.SelectedItemUI.InventoryItem != null) return;
        
        if (itemUI == _itemUI) {
            switch (_tileType) {
                case TileType.Helmet:
                    Player.Inventory.Helmet = null;
                    break;
                case TileType.Chestplate:
                    Player.Inventory.Chestplate = null;
                    break;
                case TileType.Leggings:
                    Player.Inventory.Leggings = null;
                    break;
                case TileType.Boots:
                    Player.Inventory.Boots = null;
                    break;
                case TileType.Amulet:
                    Player.Inventory.Amulet = null;
                    break;
                case TileType.Ring:
                    Player.Inventory.Ring = null;
                    break;
                case TileType.Weapon:
                    Player.Inventory.Weapon = null;
                    break;
            }

            IsEmpty = true;
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

        
        try {
            if (_tileType == TileType.Helmet) {
                if (SelectedInventoryItem.ItemData is not HelmetItemData helmetItemData) {
                    return;
                }

                Player.Inventory.Helmet = helmetItemData;
            } else if (_tileType == TileType.Chestplate) {
                if (SelectedInventoryItem.ItemData is not ChestplateItemData chestplateItemData) {
                    return;
                }

                Player.Inventory.Chestplate = chestplateItemData;
            } else if (_tileType == TileType.Leggings) {
                if (SelectedInventoryItem.ItemData is not LeggingsItemData leggingsItemData) {
                    return;
                }

                Player.Inventory.Leggings = leggingsItemData;
            } else if (_tileType == TileType.Boots) {
                if (SelectedInventoryItem.ItemData is not BootsItemData bootsItemData) {
                    return;
                }

                Player.Inventory.Boots = bootsItemData;
            } else if (_tileType == TileType.Ring) {
                if (SelectedInventoryItem.ItemData is not RingItemData ringItemData) {
                    return;
                }

                Player.Inventory.Ring = ringItemData;
            } else if (_tileType == TileType.Amulet) {
                if (SelectedInventoryItem.ItemData is not AmuletItemData amuletItemData) {
                    return;
                }

                Player.Inventory.Amulet = amuletItemData;
            } else if (_tileType == TileType.Weapon) {
                if (SelectedInventoryItem.ItemData is not WeaponItemData weaponItemData) {
                    return;
                }

                Player.Inventory.Weapon = weaponItemData;
            }
        } catch (System.Exception e) {
            Debug.Log(e);
            return;
        }

        createItemUI(SelectedInventoryItem);

        EventBus.ItemPlacedEvent?.Invoke();
        IsEmpty = false;
        Player.UICanvas.SelectedItemUI.InventoryItem = null;
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
