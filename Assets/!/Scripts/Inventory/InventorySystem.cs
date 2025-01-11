using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [Serializable]
    public class InventoryItem {
        public Item item;
        public int amount;
        public Vector2Int position;

        public Vector2Int Size {
            get {
                return item.Size;
            }
        }

        public ItemUI ItemUI { get; set; }
        public RectTransform RectTransform { get; set; }
    }

    [SerializeField] private Vector2Int inventorySize;
    // TODO: Handle side effects
    public Vector2Int InventorySize { get => inventorySize; set => inventorySize = value; }

    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();

    public bool DoesFitWithin(Vector2Int position, Vector2Int size) {
        // Position is out of bounds
        if(!IsWithinBounds(position)) {
            return false;
        }

        // Size is out of bounds
        if(!IsWithinBounds(size)) {
            return false;
        }

        foreach(var inventoryItem in inventory) {
            var minX = inventoryItem.position.x;
            var maxX = inventoryItem.position.x + inventoryItem.item.Size.x;

            var minY = inventoryItem.position.y;
            var maxY = inventoryItem.position.y + inventoryItem.item.Size.y;

            if(position.x >= minX && position.x < maxX && position.y >= minY && position.y < maxY) {
                return false;
            }
        }

        return true;
    }

    public bool CanBeAdded(Item item, Vector2Int position) {
        return CanBeAdded(item, 1, position);
    }

    public bool CanBeAdded(Item item, int quantity, Vector2Int position) {
        var inventoryItem = GetInventoryItem(position);
        if(inventoryItem == null) {
            return DoesFitWithin(position, item.Size);
        }

        if(!inventoryItem.item.Equals(item)) {
            return false;
        }

        if(inventoryItem.amount + quantity > inventoryItem.item.MaxQuantity) {
            return false;
        }

        return true;
    }

    public bool IsWithinBounds(Vector2Int position) {
        return position.x >= 0 && position.y >= 0 && position.x < inventorySize.x && position.y < inventorySize.y;
    }

    // This will throw an exception if the position is out of bounds
    // Use IsWithinBounds to check if the position is valid
    public InventoryItem GetInventoryItem(Vector2Int position) {
        foreach(var inventoryItem in inventory) {
            var minX = inventoryItem.position.x;
            var maxX = inventoryItem.position.x + inventoryItem.item.Size.x;

            var minY = inventoryItem.position.y;
            var maxY = inventoryItem.position.y + inventoryItem.item.Size.y;

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

        return addItem(item, amount, position);
    }

    private bool addItem(Item item, int amount, Vector2Int position) {
        var inventoryItem = GetInventoryItem(position);
        if(inventoryItem != null) {
            if(!inventoryItem.item.Equals(item)) {
                return false;
            }
    
            if(inventoryItem.amount + amount >= inventoryItem.item.MaxQuantity) {
                return false;
            }

            inventoryItem.amount += amount;
            return true;
        }

        inventoryItem = new InventoryItem {
            item = item,
            amount = amount,
            position = position
        };

        inventory.Add(inventoryItem);

        // debugLogInventory();

        return true;
    }

    public bool AddItem(Item item, Vector2Int position) {
        return AddItem(item, 1, position);
    }

    // Adds item anywhere in the inventory
    // Searches for space by columns then rows
    public bool AddItem(Item item, int amount) {
        for(int y = 0; y < inventorySize.y; y++) {
            for(int x = 0; x < inventorySize.x; x++) {
                if(CanBeAdded(item, new Vector2Int(x, y))) {
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

    // Returns true if the item was removed
    public bool RemoveItemAt(Vector2Int position) {
        if(!IsWithinBounds(position)) {
            return false;
        }

        var inventoryItem = GetInventoryItem(position);
        if(inventoryItem == null) {
            return false;
        }

        inventory.Remove(inventoryItem);

        return true;
    }

    // Returns true if the item was removed
    public bool RemoveInventoryItem(InventoryItem inventoryItem) {
        return inventory.Remove(inventoryItem);
    }

    public List<InventoryItem> GetItems() {
        return inventory;
    }

    public bool CanBeSafelyResizedTo(Vector2Int newSize) {
        var outOfBoundsItems = GetOutOfBoundsItemsIfResizedTo(newSize);
        return outOfBoundsItems.Count == 0;
    }

    public List<InventoryItem> GetOutOfBoundsItemsIfResizedTo(Vector2Int newSize) {
        var outOfBoundsItems = new List<InventoryItem>();

        foreach(var inventoryItem in inventory) {
            var minX = inventoryItem.position.x;
            var maxX = inventoryItem.position.x + inventoryItem.item.Size.x;

            var minY = inventoryItem.position.y;
            var maxY = inventoryItem.position.y + inventoryItem.item.Size.y;

            if(newSize.x < minX || newSize.x > maxX || newSize.y < minY || newSize.y > maxY) {
                outOfBoundsItems.Add(inventoryItem);
            }
        }

        return outOfBoundsItems;
    }

        private void debugLogInventory() {
        string s = "\n  ";

        for(int y = 0; y < inventorySize.y; y++) {
            s += "-";
        }

        s += "\n";
        
        for(int y = 0; y < inventorySize.y; y++) {
            s += "|";
            for(int x = 0; x < inventorySize.x; x++) {
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