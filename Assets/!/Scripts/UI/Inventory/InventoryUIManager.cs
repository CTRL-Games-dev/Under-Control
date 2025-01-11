using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _itemPrefab;

    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject _itemHolder;

    [SerializeField] private InventorySystem _playerInventory;
    
    private void Start() {
        _gridManager.InventoryWidth = _playerInventory.InventorySize.x;
        _gridManager.InventoryHeight = _playerInventory.InventorySize.y;
        _gridManager.SetupGrid();
        
        var inventory = _playerInventory.GetItems();

        foreach (var inventoryItem in inventory) {
            var itemGameObject = Instantiate(_itemPrefab, _itemHolder.transform);
            inventoryItem.ItemUI = itemGameObject.GetComponent<ItemUI>();
            inventoryItem.RectTransform = itemGameObject.GetComponent<RectTransform>();
            
            itemGameObject.name = inventoryItem.item.DisplayName;
            itemGameObject.GetComponent<ItemUI>().SetupItem(inventoryItem, GridManager.TileSize, _gridManager.PlaceItem(new Vector2Int(inventoryItem.position.x, inventoryItem.position.y), inventoryItem.Size));
        }
    }
}
