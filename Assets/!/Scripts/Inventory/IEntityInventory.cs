using System.Collections.Generic;
using UnityEngine;

// Inventory that is attached to an entity - MonoBehaviour
public abstract class EntityInventory : MonoBehaviour {
    public abstract Vector2Int Size { get; }
    public abstract bool AddItem(ItemData itemData, int amount, Vector2Int position, bool rotated = false);
    public abstract bool AddItem(ItemData itemData, int amount, Vector2Int position);
    public abstract bool AddItem(ItemData itemData, int amount);
    public abstract bool AddItem(ItemData itemData);
    public abstract InventoryItem GetInventoryItem(Vector2Int position);
    public abstract List<InventoryItem> GetItems();
    public abstract bool IsWithinBounds(Vector2Int position);
    public abstract bool FitsWithinBounds(Vector2Int position, Vector2Int size);
    public abstract bool CanBeAdded(ItemData itemData, int quantity, Vector2Int position);
    public abstract bool RemoveInventoryItem(InventoryItem inventoryItem);
    public abstract bool RemoveItemAt(Vector2Int position);
    public abstract void Clear();
}