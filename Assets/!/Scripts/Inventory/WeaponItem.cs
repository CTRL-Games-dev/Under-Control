using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public float Damage;
    public float Range;
    public float Cooldown;
}