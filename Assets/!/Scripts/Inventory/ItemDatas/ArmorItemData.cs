using System;
using UnityEngine;

public abstract class ArmorItemData : ItemData {
    [Serializable]
    public struct DamageResistance {
        public DamageType DamageType;

        [Range(0, 1)]
        public float Resistance;
    }
    
    public DamageResistance[] DamageResistances;
}