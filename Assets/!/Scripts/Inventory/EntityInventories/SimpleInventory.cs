using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInventory : EntityInventory
{
    [SerializeField]
    public ItemContainer ItemContainer;
    public override Vector2Int Size => ItemContainer.Size;

    // Does not guarantee that inventory WAS indeed changed
    public UnityEvent OnInventoryChanged;

    public override bool AddItem(ItemData itemData, int amount, Vector2Int position, float powerScale = 1, bool rotated = false) {
        bool wasAdded = ItemContainer.AddItem(itemData, amount, position, powerScale, rotated);
        if (wasAdded) OnInventoryChanged?.Invoke();
        return wasAdded;
    }

    public override bool AddItem(ItemData itemData, int amount = 1, float powerScale = 1) {
        bool wasAdded = ItemContainer.AddItem(itemData, amount, powerScale);
        if (wasAdded) OnInventoryChanged?.Invoke();
        return wasAdded;
    }

    public override InventoryItem GetInventoryItem(Vector2Int position) {
        return ItemContainer.GetInventoryItem(position);
    }

    public override List<InventoryItem> GetItems() {
        return ItemContainer.GetItems();
    }

    public override bool IsWithinBounds(Vector2Int position) {
        return ItemContainer.IsWithinBounds(position);
    }

    public override bool FitsWithinBounds(Vector2Int position, Vector2Int size) {
        return ItemContainer.FitsWithinBounds(position, size);
    }

    public override bool CanBeAdded(ItemData itemData, int quantity, Vector2Int position, float powerScale) {
        return ItemContainer.CanBeAdded(itemData, quantity, position, powerScale);
    }

    public override bool RemoveInventoryItem(InventoryItem inventoryItem) {
        bool wasRemoved = ItemContainer.RemoveInventoryItem(inventoryItem);
        if (wasRemoved) OnInventoryChanged?.Invoke();
        return wasRemoved;
    }

    public override bool RemoveItemAt(Vector2Int position) {
        bool wasRemoved = ItemContainer.RemoveItemAt(position);
        if (wasRemoved) OnInventoryChanged?.Invoke();
        return wasRemoved;
    }

    public override void Clear() {
        ItemContainer.Clear();
        OnInventoryChanged?.Invoke();
    }
}