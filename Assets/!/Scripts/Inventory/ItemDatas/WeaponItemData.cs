using UnityEngine;

[CreateAssetMenu(fileName = "SO_Wpn_Weapon", menuName = "Items/Weapon")]
public class WeaponItemData : ItemData {
    public float LightDamageMin;
    public float LightDamageMax;

    public float HeavyDamageMin;
    public float HeavyDamageMax;

    public DamageType DamageType;
    public Weapon WeaponPrefab;
    public WeaponType WeaponType;
}