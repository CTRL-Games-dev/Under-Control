using System;
using UnityEngine;

[Serializable]
public class InventoryItem {
    public ItemData ItemData;
    public int Amount;
    public Vector2Int Position;

    [Range(0, 2)]
    public float PowerScale;
    public bool Rotated;

    public Vector2Int Size { get => ItemData.Size; }

    public ItemUI ItemUI { get; set; }
    public RectTransform RectTransform { get; set; }

    public InventoryItem<T> As<T>() where T : ItemData {
        if(TryAs(out InventoryItem<T> inventoryItem)) {
            return inventoryItem;
        }

        throw new InvalidCastException();
    }

    public bool TryAs<T>(out InventoryItem<T> inventoryItem) where T : ItemData {
        if(ItemData is T itemData) {
            inventoryItem = new InventoryItem<T> {
                ItemData = itemData,
                Amount = Amount,
                Position = Position,
                PowerScale = PowerScale,
                Rotated = Rotated
            };

            inventoryItem.ItemUI = ItemUI;
            inventoryItem.RectTransform = RectTransform;

            return true;
        }

        inventoryItem = default;
        return false;
    }
}

[Serializable]
public class InventoryItem<T> where T : ItemData {
    public T ItemData;
    public int Amount;
    public Vector2Int Position;

    [Range(0, 2)]
    public float PowerScale;
    public bool Rotated;

    public Vector2Int Size { get => ItemData.Size; }

    public ItemUI ItemUI { get; set; }
    public RectTransform RectTransform { get; set; }

    public static implicit operator InventoryItem(InventoryItem<T> inventoryItem) {
        InventoryItem newInventoryItem = new InventoryItem {
            ItemData = inventoryItem.ItemData,
            Amount = inventoryItem.Amount,
            Position = inventoryItem.Position,
            PowerScale = inventoryItem.PowerScale,
            Rotated = inventoryItem.Rotated
        };

        newInventoryItem.ItemUI = inventoryItem.ItemUI;
        newInventoryItem.RectTransform = inventoryItem.RectTransform;

        return newInventoryItem;
    }
}