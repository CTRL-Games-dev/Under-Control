using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmorItemData", menuName = "Items/Armor")]
public class ArmorItemData : ItemData {
    // [Serializable]
    // public struct DamageResistance {
    //     public DamageType DamageType;

    //     [Range(0, 1)]
    //     public float Resistance;
    // }
    
    // [SerializeField]
    // public DamageResistance[] DamageResistances;
    public int Armor;
    public Material Material;
}