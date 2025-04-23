using UnityEngine;
[CreateAssetMenu(fileName = "SO_Chest_Item", menuName = "Items/ChestItemData")]
public class ChestItemData : ScriptableObject
{
    public ItemData item;
    public float MaxQuantity;
    public float MinInfluence;
    public float MinInfluenceDelta;
    [Range(0f, 1f)]
    public float ChanceToSpawn;
}