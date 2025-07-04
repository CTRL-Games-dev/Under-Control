using System;
using UnityEngine;

[Serializable]
public struct SpawnItemData
{
    public ItemData item;
    public float MaxQuantity;
    public float MinInfluence;
    public float MinInfluenceDelta;
}