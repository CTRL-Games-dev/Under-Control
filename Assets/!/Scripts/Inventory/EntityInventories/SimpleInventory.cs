using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInventory : EntityInventory
{
    [SerializeField]
    public ItemContainer ItemContainer;
    public override Vector2Int Size => ItemContainer.Size;

    public override bool AddItem(ItemData itemData, int amount, Vector2Int position, bool rotated = false) {
        return ItemContainer.AddItem(itemData, amount, position, rotated);
    }

    public override bool AddItem(ItemData itemData, int amount, Vector2Int position) {
        return ItemContainer.AddItem(itemData, amount, position);
    }

    public override bool AddItem(ItemData itemData, int amount) {
        return ItemContainer.AddItem(itemData, amount);
    }

    public override bool AddItem(ItemData itemData) {
        return ItemContainer.AddItem(itemData);
    }

    public override InventoryItem GetInventoryItem(Vector2Int position) {
        return ItemContainer.GetInventoryItem(position);
    }

    public override  List<InventoryItem> GetItems() {
        return ItemContainer.GetItems();
    }

    public override bool IsWithinBounds(Vector2Int position) {
        return ItemContainer.IsWithinBounds(position);
    }

    public override bool FitsWithinBounds(Vector2Int position, Vector2Int size) {
        return ItemContainer.FitsWithinBounds(position, size);
    }

    public override bool CanBeAdded(ItemData itemData, int quantity, Vector2Int position) {
        return ItemContainer.CanBeAdded(itemData, quantity, position);
    }

    public override bool RemoveInventoryItem(InventoryItem inventoryItem) {
        return ItemContainer.RemoveInventoryItem(inventoryItem);
    }

    public override bool RemoveItemAt(Vector2Int position) {
        return ItemContainer.RemoveItemAt(position);
    }

    public override void Clear() {
        ItemContainer.Clear();
    }
}