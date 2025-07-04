using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemContainer
{
    [SerializeField] private Vector2Int _size;
    // TODO: Handle side effects
    public Vector2Int Size { get => _size; set => _size = value; }

    [SerializeField]
    private List<InventoryItem> _items = new List<InventoryItem>();

    public ItemContainer(int width, int height) {
        if(width < 1 || height < 1) {
            throw new ArgumentOutOfRangeException("Inventory size must be greater than 0");
        }

        _size = new Vector2Int(width, height);
    }

    public bool DoesFitWithin(Vector2Int position, Vector2Int size) {
        // Position is out of bounds
        if(!IsWithinBounds(position)) {
            return false;
        }

        // Item bounds are out of bounds
        // Size 1 would be offset 0 so we need to check bounds for size - 1
        if(!IsWithinBounds(position + size - Vector2Int.one)) {
            return false;
        }

        foreach(var inventoryItem in _items) {
            // Illegal bounds = rectangle in which item can't be placed
            var illegalBoundsMinX = inventoryItem.Position.x - size.x + 1;
            var illegalBoundsMaxX = inventoryItem.Position.x + (inventoryItem.Rotated ? inventoryItem.ItemData.Size.y : inventoryItem.ItemData.Size.x) - 1;

            var illegalBoundsMinY = inventoryItem.Position.y - size.y + 1;
            var illegalBoundsMaxY = inventoryItem.Position.y + (inventoryItem.Rotated ? inventoryItem.ItemData.Size.x : inventoryItem.ItemData.Size.y) - 1;

            if(position.x >= illegalBoundsMinX && position.x <= illegalBoundsMaxX && position.y >= illegalBoundsMinY && position.y <= illegalBoundsMaxY) {
                return false;
            }
        }

        return true;
    }

    public bool CanBeAdded(ItemData itemData, int quantity, Vector2Int position, float powerScale, bool rotated = false) {
        var inventoryItem = GetInventoryItem(position);
        if(inventoryItem == null) {
            Vector2Int size = itemData.Size;
            if(rotated) {
                size = new Vector2Int(size.y, size.x);
            }

            return DoesFitWithin(position, size);
        }

        if(!inventoryItem.ItemData.Equals(itemData)) {
            return false;
        }

        if(inventoryItem.Amount + quantity > inventoryItem.ItemData.MaxQuantity) {
            return false;
        }

        if(!Mathf.Approximately(inventoryItem.PowerScale, powerScale)) {
            return false;
        }

        return true;
    }

    public bool IsWithinBounds(Vector2Int position) {
        return position.x >= 0 && position.y >= 0 && position.x < _size.x && position.y < _size.y;
    }

    public InventoryItem GetFirstInventoryItem(ItemData itemData) {
        foreach(var inventoryItem in _items) {
            if(inventoryItem.ItemData.Equals(itemData)) {
                return inventoryItem;
            }
        }

        return null;
    }


    // This will throw an exception if the position is out of bounds
    // Use IsWithinBounds to check if the position is valid
    public InventoryItem GetInventoryItem(Vector2Int position) {
        foreach(var inventoryItem in _items) {
            var minX = inventoryItem.Position.x;
            var maxX = inventoryItem.Position.x + (inventoryItem.Rotated ? inventoryItem.ItemData.Size.y : inventoryItem.ItemData.Size.x);

            var minY = inventoryItem.Position.y;
            var maxY = inventoryItem.Position.y + (inventoryItem.Rotated ? inventoryItem.ItemData.Size.x : inventoryItem.ItemData.Size.y);

            if(position.x >= minX && position.x < maxX && position.y >= minY && position.y < maxY) {
                return inventoryItem;
            }
        }

        return null;
    }

    // Returns true if the item was added
    public bool AddItem(ItemData itemData, int amount, Vector2Int position, float powerScale = 1, bool rotated = false) {
        if(!IsWithinBounds(position)) {
            throw new Exception("Position is out of bounds");
        }

        if(!CanBeAdded(itemData, amount, position, powerScale, rotated)) {
            return false;
        }

        return addItem(itemData, amount, position, powerScale, rotated);
    }

    // Adds item anywhere in the inventory
    // Searches for space by columns then rows
    public bool AddItem(ItemData itemData, int amount = 1, float powerScale = 1) {
        for(int y = 0; y < _size.y; y++) {
            for(int x = 0; x < _size.x; x++) {
                if(CanBeAdded(itemData, amount, new Vector2Int(x, y), powerScale, false)) {
                    return addItem(itemData, amount, new Vector2Int(x, y), powerScale, false);
                }
            }
        }

        // Try rotated
        for(int y = 0; y < _size.y; y++) {
            for(int x = 0; x < _size.x; x++) {
                if(CanBeAdded(itemData, amount, new Vector2Int(x, y), powerScale, true)) {
                    return addItem(itemData, amount, new Vector2Int(x, y), powerScale, true);
                }
            }
        }

        return false;
    }

    public bool AddItem(ItemData itemData, int amount = 1, float powerScale = 1, bool rotated = false) {
        for(int y = 0; y < _size.y; y++) {
            for(int x = 0; x < _size.x; x++) {
                if(CanBeAdded(itemData, amount, new Vector2Int(x, y), powerScale, rotated)) {
                    return addItem(itemData, amount, new Vector2Int(x, y), powerScale, rotated);
                }
            }
        }

        return false;
    }

    private bool addItem(ItemData itemData, int amount, Vector2Int position, float powerScale = 1, bool rotated = false) {
        var inventoryItem = GetInventoryItem(position);
        if(inventoryItem != null) {
            if(!inventoryItem.ItemData.Equals(itemData)) {
                return false;
            }
    
            if(inventoryItem.Amount + amount > inventoryItem.ItemData.MaxQuantity) {
                return false;
            }

            inventoryItem.Amount += amount;

            return true;
        }

        inventoryItem = new InventoryItem {
            ItemData = itemData,
            Amount = amount,
            Position = position,
            PowerScale = powerScale,
            Rotated = rotated
        };

        _items.Add(inventoryItem);

        return true;
    }

    // Returns true if the item was removed
    public bool RemoveItemAt(Vector2Int position) {
        if(!IsWithinBounds(position)) {
            return false;
        }

        var inventoryItem = GetInventoryItem(position);
        if(inventoryItem == null) {
            return false;
        }

        return _items.Remove(inventoryItem);
    }

    // Returns true if the item was removed
    public bool RemoveInventoryItem(InventoryItem inventoryItem) {
        if(!_items.Remove(inventoryItem)) {
            return false;
        }

        inventoryItem.ItemUI = null;
        inventoryItem.RectTransform = null;

        return true;
    }



    private InventoryItem GetFirstItemData(ItemData itemData, int amount) {
        foreach(var inventoryItem in _items) {
            if(inventoryItem.ItemData.Equals(itemData)) {
                if(inventoryItem.Amount >= amount) {
                    return inventoryItem;
                }
            }
        }

        return null;
    }

    public List<InventoryItem> GetItems() {
        return _items;
    }

    public bool CanBeSafelyResizedTo(Vector2Int newSize) {
        var outOfBoundsItems = GetOutOfBoundsItemsIfResizedTo(newSize);
        return outOfBoundsItems.Count == 0;
    }

    public List<InventoryItem> GetOutOfBoundsItemsIfResizedTo(Vector2Int newSize) {
        var outOfBoundsItems = new List<InventoryItem>();

        foreach(var inventoryItem in _items) {
            var minX = inventoryItem.Position.x;
            var maxX = inventoryItem.Position.x + inventoryItem.ItemData.Size.x;

            var minY = inventoryItem.Position.y;
            var maxY = inventoryItem.Position.y + inventoryItem.ItemData.Size.y;

            if(newSize.x < minX || newSize.x > maxX || newSize.y < minY || newSize.y > maxY) {
                outOfBoundsItems.Add(inventoryItem);
            }
        }

        return outOfBoundsItems;
    }

    public void Clear() {
        _items.Clear();
    }

    public bool HasItemData(ItemData itemData, int amount){
        foreach(var inventoryItem in _items) {
            if(inventoryItem.ItemData.Equals(itemData)) {
                if(inventoryItem.Amount >= amount) {
                    return true;
                }
            }
        }

        return false;
    }
}