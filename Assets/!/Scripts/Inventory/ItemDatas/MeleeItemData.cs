using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Item", menuName = "Items/Melee Item")]
public class MeleeItemData : WeaponItemData {
    public float Damage;
    public float Range;
    public float Cooldown;
}