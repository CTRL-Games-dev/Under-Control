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
}