using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInventory : EntityInventory
{
    [SerializeField]
    public ItemContainer itemContainer;
    public override Vector2Int Size => itemContainer.Size;

    public override bool AddItem(ItemData itemData, int amount, Vector2Int position, bool rotated = false) {
        return itemContainer.AddItem(itemData, amount, position, rotated);
    }

    public override bool AddItem(ItemData itemData, int amount, Vector2Int position) {
        return itemContainer.AddItem(itemData, amount, position);
    }

    public override bool AddItem(ItemData itemData, int amount) {
        return itemContainer.AddItem(itemData, amount);
    }

    public override bool AddItem(ItemData itemData) {
        return itemContainer.AddItem(itemData);
    }

    public override InventoryItem GetInventoryItem(Vector2Int position) {
        return itemContainer.GetInventoryItem(position);
    }

    public override  List<InventoryItem> GetItems() {
        return itemContainer.GetItems();
    }

    public override bool IsWithinBounds(Vector2Int position) {
        return itemContainer.IsWithinBounds(position);
    }

    public override bool FitsWithinBounds(Vector2Int position, Vector2Int size) {
        return itemContainer.FitsWithinBounds(position, size);
    }

    public override bool CanBeAdded(ItemData itemData, int quantity, Vector2Int position) {
        return itemContainer.CanBeAdded(itemData, quantity, position);
    }

    public override bool RemoveInventoryItem(InventoryItem inventoryItem) {
        return itemContainer.RemoveInventoryItem(inventoryItem);
    }

    public override bool RemoveItemAt(Vector2Int position) {
        return itemContainer.RemoveItemAt(position);
    }

    public override void Clear() {
        itemContainer.Clear();
    }
}