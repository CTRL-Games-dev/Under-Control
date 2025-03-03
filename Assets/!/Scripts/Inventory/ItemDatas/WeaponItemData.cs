using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Item", menuName = "Items/Melee Item")]
public class WeaponItemData : ItemData {
    public float DamageMin;
    public float DamageMax;
    public DamageType DamageType;
}