using UnityEngine;

[CreateAssetMenu(fileName = "SO_Wpn_Weapon", menuName = "Items/Weapon")]
public class WeaponItemData : ItemData {
    public float DamageMin;
    public float DamageMax;
    public DamageType DamageType;
    public Weapon WeaponPrefab;
    public WeaponType WeaponType;
}