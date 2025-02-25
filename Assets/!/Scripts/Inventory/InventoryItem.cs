using System;
using UnityEngine;

[Serializable]
public class InventoryItem {
    public ItemData ItemData;
    public int Amount;
    public Vector2Int Position;

    public Vector2Int Size { get => ItemData.Size; }

    public bool Rotated;
    public ItemUI ItemUI { get; set; }
    public RectTransform RectTransform { get; set; }
}