using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InventorySystem;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _itemPrefab;

    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject _itemHolder;

    [SerializeField] private InventorySystem _playerInventory;
    
    private List<InventoryItem> _inventory;

    private float _tileSize;


    private void Start() {
        _gridManager.InventoryWidth = _playerInventory.InventorySize.x;
        _gridManager.InventoryHeight = _playerInventory.InventorySize.y;
        _gridManager.SetupGrid();

        _tileSize = _gridManager.TileSize;
        
        _inventory = _playerInventory.GetItems();

        setupItems();
    }

    private void setupItems() {
        foreach (InventoryItem inventoryItem in _inventory) {
            var itemGameObject = Instantiate(_itemPrefab, _itemHolder.transform);
            inventoryItem.GameObject = itemGameObject;

            itemGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_tileSize * inventoryItem.Size.x, _tileSize * inventoryItem.Size.y);
            itemGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(_tileSize * inventoryItem.position.x, -_tileSize * inventoryItem.position.y);
            itemGameObject.GetComponent<Image>().sprite = inventoryItem.item.Icon;

            _gridManager.TryPlaceItem(new Vector2Int(inventoryItem.position.x, inventoryItem.position.y), inventoryItem.Size);
        }
    }

}

