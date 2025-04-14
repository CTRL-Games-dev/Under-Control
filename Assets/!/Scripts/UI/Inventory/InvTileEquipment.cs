using UnityEngine;

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
                    inventoryItem.ItemData = Player.Inventory.Armor;
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

            case TileType.Weapon:
                if (Player.Inventory.Weapon != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Inventory.Weapon;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break; 
            
            case TileType.Consumeable1:
                if (Player.Instance.ConsumableItemOne != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Instance.ConsumableItemOne;
                    createItemUI(inventoryItem);
                    IsEmpty = false;
                }
                break;
            
            case TileType.Consumeable2:
                if (Player.Instance.ConsumableItemTwo != null) {
                    InventoryItem inventoryItem = new();
                    inventoryItem.ItemData = Player.Instance.ConsumableItemTwo;
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
                    break;
                case TileType.Consumeable2:
                    Player.Instance.ConsumableItemTwo = null;
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
            if (_tileType == TileType.Armor) {
                if (SelectedInventoryItem.ItemData is not ArmorItemData armorItemData) {
                    return;
                }

                Player.Inventory.Armor = armorItemData;
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
            } else if (_tileType == TileType.Consumeable1) {
                if (SelectedInventoryItem.ItemData is not ConsumableItemData consumableItemData) {
                    return;
                }

                Player.Instance.ConsumableItemOne = consumableItemData;
            } else if (_tileType == TileType.Consumeable2) {
                if (SelectedInventoryItem.ItemData is not ConsumableItemData consumableItemData) {
                    return;
                }

                Player.Instance.ConsumableItemTwo = consumableItemData;
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
