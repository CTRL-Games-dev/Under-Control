using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public float damage;
    public float range;
    public float cooldown;
}