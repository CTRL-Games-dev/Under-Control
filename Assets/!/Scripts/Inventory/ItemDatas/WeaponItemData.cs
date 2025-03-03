using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Item", menuName = "Items/Melee Item")]
public class WeaponItemData : ItemData {
    public float Damage;
    public DamageType DamageType;
    public float Range;
    public float Cooldown;
}