using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInventory : MonoBehaviour, IEntityInventory
{
    [SerializeField]
    public ItemContainer itemContainer;
    public Vector2Int Size => itemContainer.Size;

    public bool AddItem(ItemData itemData, int amount, Vector2Int position, bool rotated = false) {
        return itemContainer.AddItem(itemData, amount, position, rotated);
    }

    public bool AddItem(ItemData itemData, int amount, Vector2Int position) {
        return itemContainer.AddItem(itemData, amount, position);
    }

    public bool AddItem(ItemData itemData, Vector2Int position) {
        return itemContainer.AddItem(itemData, position);
    }

    public bool AddItem(ItemData itemData, int amount) {
        return itemContainer.AddItem(itemData, amount);
    }

    public bool AddItem(ItemData itemData) {
        return itemContainer.AddItem(itemData);
    }

    public bool DropItem(InventoryItem item) {
        bool ok = itemContainer.RemoveInventoryItem(item);
        if(!ok) {
            return false;
        }

        ItemEntityManager.Instance.SpawnItemEntity(item.ItemData, item.Amount, transform.position);

        return true;
    }

    public InventoryItem GetInventoryItem(Vector2Int position) {
        return itemContainer.GetInventoryItem(position);
    }

    public List<InventoryItem> GetItems() {
        return itemContainer.GetItems();
    }

    public virtual List<InventoryItem> GetAdditionalSlots() {
        return new List<InventoryItem>();
    }

    public bool IsWithinBounds(Vector2Int position) {
        return itemContainer.IsWithinBounds(position);
    }

    public bool FitsWithinBounds(Vector2Int position, Vector2Int size) {
        return itemContainer.FitsWithinBounds(position, size);
    }

    public bool CanBeAdded(ItemData itemData, int quantity, Vector2Int position) {
        return itemContainer.CanBeAdded(itemData, quantity, position);
    }

    public bool RemoveInventoryItem(InventoryItem inventoryItem) {
        return itemContainer.RemoveInventoryItem(inventoryItem);
    }

    public bool RemoveItemAt(Vector2Int position) {
        return itemContainer.RemoveItemAt(position);
    }

    public virtual void Clear() {
        itemContainer.Clear();
    }
}