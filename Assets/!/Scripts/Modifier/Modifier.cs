using System;
using UnityEngine;

[Serializable]
public struct Modifier {
    public StatType StatType;
    public float Value;
    public ModifierType Type;
    public Modifier(StatType statType, float value, ModifierType type = ModifierType.ADDITIVE) {
        this.StatType = statType;
        this.Value = value;
        this.Type = type;
    }

    public float Calculate(float baseValue) {
        switch(Type) {
            case ModifierType.ADDITIVE:
                return baseValue + Value;
            case ModifierType.MULTIPLICATIVE:
                return baseValue * Value;
            case ModifierType.PERCENT_CHANGE:
                //  20 => value * 1.2
                // -20 => value * 0.8
                return baseValue * (1 + Value / 100);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override string ToString() {
        string valueString;
        switch(Type) {
            case ModifierType.ADDITIVE:
                if (Value > 0) {
                    valueString = $"+{Value}";
                } else {
                    valueString = $"{Value}";
                }

                break;
            case ModifierType.MULTIPLICATIVE:
                valueString = $"{Value}x";
                break;
            case ModifierType.PERCENT_CHANGE:
                if (Value > 0) {
                    valueString = $"+{Value}%";
                } else {
                    valueString = $"{Value}%";
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return $"{valueString} {StatType.GetDisplayName()}";
    }

    public string ToRichTextString() {
        ModifierImpact impact = GetImpact();

        string color;
        switch(impact) {
            case ModifierImpact.POSITIVE:
                color = "#00FF00";
                break;
            case ModifierImpact.NEUTRAL:
                color = "#FFFFFF";
                break;
            case ModifierImpact.NEGATIVE:
                color = "#FF0000";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return $"<color={color}>{ToString()}</color>";
    }

    public ModifierImpact GetImpact() {
        switch(StatType) {
            case StatType.HEALTH:
                if (Value > 0) {
                    return ModifierImpact.POSITIVE;
                } else if (Value < 0) {
                    return ModifierImpact.NEGATIVE;
                } else {
                    return ModifierImpact.NEUTRAL;
                }
            case StatType.MAX_HEALTH:
                if (Value > 0) {
                    return ModifierImpact.POSITIVE;
                } else if (Value < 0) {
                    return ModifierImpact.NEGATIVE;
                } else {
                    return ModifierImpact.NEUTRAL;
                }
            case StatType.REGEN_RATE:
                if (Value > 0) {
                    return ModifierImpact.POSITIVE;
                } else if (Value < 0) {
                    return ModifierImpact.NEGATIVE;
                } else {
                    return ModifierImpact.NEUTRAL;
                }
            default:
                Debug.LogError($"Requested impact for stat type {StatType} but it is not implemented, returning NEUTRAL");
                return ModifierImpact.NEUTRAL;
        }
    }
}