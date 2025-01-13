using System;
using UnityEngine;

[Serializable]
public class Stat {
    public StatType StatType;

    [SerializeField]
    private float _raw;
    public float Raw { get => _raw; protected set => _raw = value; }

    [SerializeField]
    private float _adjusted;
    public float Adjusted { get => _adjusted; protected set => _adjusted = value; }

    public Stat(StatType statType, float initValue) {
        this.StatType = statType;
        Raw = initValue;
        Adjusted = initValue;
    }

    public float Recalculate(ModifierSystem modifierSystem) {
        Adjusted = modifierSystem.CalculateForStatType(StatType, Raw);
        return Adjusted;
    }

    public static implicit operator float(Stat stat) {
        return stat.Adjusted;
    }
}