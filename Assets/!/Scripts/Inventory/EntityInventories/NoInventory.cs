using System.Collections.Generic;
using UnityEngine;

// Stub inventory that does nothing
// Used for entities that don't have an inventory - fauna etc
public class NoInventory : EntityInventory
{
    public override Vector2Int Size => Vector2Int.zero;

    public override bool AddItem(ItemData itemData, int amount, Vector2Int position, float powerScale = 1, bool rotated = false)
    {
        return false;
    }

    public override bool AddItem(ItemData itemData, int amount = 1, float powerScale = 1)
    {
        return false;
    }

    public override bool CanBeAdded(ItemData itemData, int quantity, Vector2Int position, float powerScale = 1)
    {
        return false;
    }
    
    public override bool FitsWithinBounds(Vector2Int position, Vector2Int size)
    {
        return false;
    }

    public override InventoryItem GetInventoryItem(Vector2Int position)
    {
        return null;
    }

    public override List<InventoryItem> GetItems()
    {
        return new List<InventoryItem>();
    }

    public override bool IsWithinBounds(Vector2Int position)
    {
        return false;
    }

    public override bool RemoveInventoryItem(InventoryItem inventoryItem)
    {
        return false;
    }

    public override bool RemoveItemAt(Vector2Int position)
    {
        return false;
    }

    public override void Clear() {}


    public override bool HasItemData(ItemData itemData, int amount = 1)
    {
        return false;
    }

    public override InventoryItem GetFirstInventoryItem(ItemData itemData)
    {
        return null;
    }
}