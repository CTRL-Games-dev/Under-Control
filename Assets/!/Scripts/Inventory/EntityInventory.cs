using System.Collections.Generic;
using UnityEngine;

// Inventory that is attached to an entity - MonoBehaviour
public abstract class EntityInventory : MonoBehaviour {
    public abstract Vector2Int Size { get; }
    public abstract bool AddItem(ItemData itemData, int amount, Vector2Int position, float powerScale = 1, bool rotated = false);
    public abstract bool AddItem(ItemData itemData, int amount = 1, float powerScale = 1);
    public abstract bool AddItem(ItemData itemData, int amount = 1, float powerScale = 1, bool rotated = false);
    public abstract InventoryItem GetInventoryItem(Vector2Int position);
    public abstract List<InventoryItem> GetItems();
    public abstract bool IsWithinBounds(Vector2Int position);
    public abstract bool FitsWithinBounds(Vector2Int position, Vector2Int size);
    public abstract bool CanBeAdded(ItemData itemData, int quantity, Vector2Int position, float powerScale = 1);
    public abstract bool RemoveInventoryItem(InventoryItem inventoryItem);
    public abstract bool RemoveItemAt(Vector2Int position);
    public abstract bool HasItemData(ItemData itemData, int amount = 1);
    public abstract InventoryItem GetFirstInventoryItem(ItemData itemData);    
    public abstract void Clear();
}