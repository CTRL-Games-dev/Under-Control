using System.Collections.Generic;
using UnityEngine;

// Stub inventory that does nothing
// Used for entities that don't have an inventory - fauna etc
public class NoInventory : MonoBehaviour, IEntityInventory
{
    public Vector2Int Size => Vector2Int.zero;

    public bool AddItem(ItemData itemData, int amount, Vector2Int position, bool rotated = false)
    {
        return false;
    }

    public bool AddItem(ItemData itemData, int amount, Vector2Int position)
    {
        return false;
    }

    public bool AddItem(ItemData itemData, int amount)
    {
        return false;
    }

    public bool AddItem(ItemData itemData)
    {
        return false;
    }

    public bool CanBeAdded(ItemData itemData, int quantity, Vector2Int position)
    {
        return false;
    }

    public bool DropItem(InventoryItem item)
    {
        return false;
    }

    public bool FitsWithinBounds(Vector2Int position, Vector2Int size)
    {
        return false;
    }

    public InventoryItem GetInventoryItem(Vector2Int position)
    {
        return null;
    }

    public List<InventoryItem> GetItems()
    {
        return new List<InventoryItem>();
    }

    public List<InventoryItem> GetAdditionalSlots()
    {
        return new List<InventoryItem>();
    }

    public bool IsWithinBounds(Vector2Int position)
    {
        return false;
    }

    public bool RemoveInventoryItem(InventoryItem inventoryItem)
    {
        return false;
    }

    public bool RemoveItemAt(Vector2Int position)
    {
        return false;
    }

    public void Clear() {}
}