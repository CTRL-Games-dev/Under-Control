using System.Collections.Generic;
using UnityEngine;

// Inventory that is attached to an entity - MonoBehaviour
public interface IEntityInventory {
    public Vector2Int Size { get; }
    public bool AddItem(ItemData itemData, int amount, Vector2Int position, bool rotated = false);
    public bool AddItem(ItemData itemData, int amount, Vector2Int position);
    public bool AddItem(ItemData itemData, int amount);
    public bool AddItem(ItemData itemData);
    public InventoryItem GetInventoryItem(Vector2Int position);
    public List<InventoryItem> GetItems();
    public bool IsWithinBounds(Vector2Int position);
    public bool FitsWithinBounds(Vector2Int position, Vector2Int size);
    public bool CanBeAdded(ItemData itemData, int quantity, Vector2Int position);
    public bool RemoveInventoryItem(InventoryItem inventoryItem);
    public bool RemoveItemAt(Vector2Int position);
    public void Clear();
}