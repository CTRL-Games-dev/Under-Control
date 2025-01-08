using System;
using UnityEngine;

[Serializable]
public class Stat {
    public StatType statType;

    [SerializeField]
    private float _raw;
    public float raw { get => _raw; protected set => _raw = value; }

    [SerializeField]
    private float _adjusted;
    public float adjusted { get => _adjusted; protected set => _adjusted = value; }

    public Stat(StatType statType, float initValue) {
        this.statType = statType;
        raw = initValue;
        adjusted = initValue;
    }

    public float Recalculate(ModifierSystem modifierSystem) {
        adjusted = modifierSystem.CalculateForStatType(statType, raw);
        return adjusted;
    }

    public static implicit operator float(Stat stat) {
        return stat.adjusted;
    }
}