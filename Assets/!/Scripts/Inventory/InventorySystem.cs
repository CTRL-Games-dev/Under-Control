using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [Serializable]
    public class InventoryItem {
        public Item Item;
        public int Amount;
        public Vector2Int Position;

        public Vector2Int Size {
            get {
                return Item.Size;
            }
        }

        public bool Rotated;
        public ItemUI ItemUI { get; set; }
        public RectTransform RectTransform { get; set; }
    }

    [SerializeField] private Vector2Int _inventorySize;
    // TODO: Handle side effects
    public Vector2Int InventorySize { get => _inventorySize; set => _inventorySize = value; }

    [SerializeField] private List<InventoryItem> _inventory = new List<InventoryItem>();

    public bool FitsWithinBounds(Vector2Int position, Vector2Int size) {
        if (position.x + size.x > InventorySize.x || position.y + size.y > InventorySize.y) {
            return false;
        }
        return true;
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

        foreach(var inventoryItem in _inventory) {
            // Illegal bounds = rectangle in which item can't be placed
            var illegalBoundsMinX = inventoryItem.Position.x - size.x + 1;
            var illegalBoundsMaxX = inventoryItem.Position.x + inventoryItem.Item.Size.x - 1;

            var illegalBoundsMinY = inventoryItem.Position.y - size.y + 1;
            var illegalBoundsMaxY = inventoryItem.Position.y + inventoryItem.Item.Size.y - 1;

            if(position.x >= illegalBoundsMinX && position.x <= illegalBoundsMaxX && position.y >= illegalBoundsMinY && position.y <= illegalBoundsMaxY) {
                return false;
            }
        }

        return true;
    }

    public bool CanBeAdded(Item item, int quantity, Vector2Int position) {
        var inventoryItem = GetInventoryItem(position);
        if(inventoryItem == null) {
            return DoesFitWithin(position, item.Size);
        }

        if(!inventoryItem.Item.Equals(item)) {
            return false;
        }

        if(inventoryItem.Amount + quantity > inventoryItem.Item.MaxQuantity) {
            return false;
        }

        return true;
    }

    public bool IsWithinBounds(Vector2Int position) {
        return position.x >= 0 && position.y >= 0 && position.x < _inventorySize.x && position.y < _inventorySize.y;
    }

    // This will throw an exception if the position is out of bounds
    // Use IsWithinBounds to check if the position is valid
    public InventoryItem GetInventoryItem(Vector2Int position) {
        foreach(var inventoryItem in _inventory) {
            var minX = inventoryItem.Position.x;
            var maxX = inventoryItem.Position.x + inventoryItem.Item.Size.x;

            var minY = inventoryItem.Position.y;
            var maxY = inventoryItem.Position.y + inventoryItem.Item.Size.y;

            if(position.x >= minX && position.x < maxX && position.y >= minY && position.y < maxY) {
                return inventoryItem;
            }
        }

        return null;
    }

    // Returns true if the item was added
    public bool AddItem(Item item, int amount, Vector2Int position) {
        if(!IsWithinBounds(position)) {
            throw new Exception("Position is out of bounds");
        }

        if(!CanBeAdded(item, amount, position)) {
            return false;
        }

        return addItem(item, amount, position);
    }

    public bool AddItem(Item item, Vector2Int position) {
        return AddItem(item, 1, position);
    }

    // Adds item anywhere in the inventory
    // Searches for space by columns then rows
    public bool AddItem(Item item, int amount) {
        for(int y = 0; y < _inventorySize.y; y++) {
            for(int x = 0; x < _inventorySize.x; x++) {
                if(CanBeAdded(item, amount, new Vector2Int(x, y))) {
                    return addItem(item, amount, new Vector2Int(x, y));
                }
            }
        }

        return false;
    }

    // Adds item anywhere in the inventory
    // Searches for space by columns then rows
    public bool AddItem(Item item) {
        return AddItem(item, 1);
    }

    private bool addItem(Item item, int amount, Vector2Int position) {
        var inventoryItem = GetInventoryItem(position);
        if(inventoryItem != null) {
            if(!inventoryItem.Item.Equals(item)) {
                return false;
            }
    
            if(inventoryItem.Amount + amount >= inventoryItem.Item.MaxQuantity) {
                return false;
            }

            inventoryItem.Amount += amount;
            return true;
        }

        inventoryItem = new InventoryItem {
            Item = item,
            Amount = amount,
            Position = position
        };

        _inventory.Add(inventoryItem);

        // debugLogInventory();

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

        _inventory.Remove(inventoryItem);

        return true;
    }

    // Returns true if the item was removed
    public bool RemoveInventoryItem(InventoryItem inventoryItem) {
        inventoryItem.ItemUI = null;
        inventoryItem.RectTransform = null;
        return _inventory.Remove(inventoryItem);
    }

    public List<InventoryItem> GetItems() {
        return _inventory;
    }

    public bool CanBeSafelyResizedTo(Vector2Int newSize) {
        var outOfBoundsItems = GetOutOfBoundsItemsIfResizedTo(newSize);
        return outOfBoundsItems.Count == 0;
    }

    public List<InventoryItem> GetOutOfBoundsItemsIfResizedTo(Vector2Int newSize) {
        var outOfBoundsItems = new List<InventoryItem>();

        foreach(var inventoryItem in _inventory) {
            var minX = inventoryItem.Position.x;
            var maxX = inventoryItem.Position.x + inventoryItem.Item.Size.x;

            var minY = inventoryItem.Position.y;
            var maxY = inventoryItem.Position.y + inventoryItem.Item.Size.y;

            if(newSize.x < minX || newSize.x > maxX || newSize.y < minY || newSize.y > maxY) {
                outOfBoundsItems.Add(inventoryItem);
            }
        }

        return outOfBoundsItems;
    }

    private void debugLogInventory() {
        string s = "\n  ";

        for(int y = 0; y < _inventorySize.y; y++) {
            s += "-";
        }

        s += "\n";
        
        for(int y = 0; y < _inventorySize.y; y++) {
            s += "|";
            for(int x = 0; x < _inventorySize.x; x++) {
                var slot = GetInventoryItem(new Vector2Int(x, y));
                if(slot != null) {
                    s += "#";
                } else {
                    s += "  ";
                }
            }
            s += "\n";
        }
        Debug.Log(s);
    }
}