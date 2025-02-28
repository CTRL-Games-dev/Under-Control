using UnityEngine;



public class InvTileEquipment : InvTile {
    public ItemType WantedItemType;
    [SerializeField] private GameObject _itemPrefab;


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
            switch (WantedItemType) {
                case ItemType.Helmet:
                    _uiCanvas.PlayerInventory.Helmet = null;
                    break;
                case ItemType.Chestplate:
                    _uiCanvas.PlayerInventory.Chestplate = null;
                    break;
                case ItemType.Leggings:
                    _uiCanvas.PlayerInventory.Leggings = null;
                    break;
                case ItemType.Boots:
                    _uiCanvas.PlayerInventory.Boots = null;
                    break;
                case ItemType.Amulet:
                    _uiCanvas.PlayerInventory.Amulet = null;
                    break;
                case ItemType.Ring:
                    _uiCanvas.PlayerInventory.Ring = null;
                    break;
                case ItemType.LeftHand:
                    _uiCanvas.PlayerInventory.UnequipLeftHand();
                    break;
                case ItemType.RightHand:
                    _uiCanvas.PlayerInventory.UnequipRightHand();
                    break;
            }

            IsEmpty = true;
            Destroy(_itemUI.gameObject);
            _itemUI = null;
        }
    }

    public void OnPointerClickEquipment() {
        if (SelectedInventoryItem == null && _itemUI == null || SelectedInventoryItem != null && _itemUI != null) return;

        if (WantedItemType == SelectedInventoryItem.ItemData.ItemType && IsEmpty) {
            switch (WantedItemType) {
                case ItemType.Helmet:
                    _uiCanvas.PlayerInventory.Helmet = SelectedInventoryItem.ItemData as HelmetItemData;
                    break;
                case ItemType.Chestplate:
                    _uiCanvas.PlayerInventory.Chestplate = SelectedInventoryItem.ItemData as ChestplateItemData;
                    break;
                case ItemType.Leggings:
                    _uiCanvas.PlayerInventory.Leggings = SelectedInventoryItem.ItemData as LeggingsItemData;
                    break;
                case ItemType.Boots:
                    _uiCanvas.PlayerInventory.Boots = SelectedInventoryItem.ItemData as BootsItemData;
                    break;
                case ItemType.Amulet:
                    _uiCanvas.PlayerInventory.Amulet = SelectedInventoryItem.ItemData as AmuletItemData;
                    break;
                case ItemType.Ring:
                    _uiCanvas.PlayerInventory.Ring = SelectedInventoryItem.ItemData as RingItemData;
                    break;
                case ItemType.LeftHand:
                    if (!_uiCanvas.PlayerInventory.EquipLeftHand(SelectedInventoryItem.ItemData as WeaponItemData)) {
                        Debug.Log("Cannot equip weapon in left hand");
                        return;
                    }
                    break;
                case ItemType.RightHand:
                    if (!_uiCanvas.PlayerInventory.EquipRightHand(SelectedInventoryItem.ItemData as WeaponItemData)) {
                        Debug.Log("Cannot equip weapon in right hand");
                        return;
                    }
                    break;
            }


            createItemUI(SelectedInventoryItem);
            EventBus.ItemPlacedEvent?.Invoke();
            IsEmpty = false;
            _uiCanvas.SelectedItemUI.InventoryItem = null;
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
