using System;
using UnityEngine;

[Serializable]
public struct Modifier {
    public const string POSITIVE_COLOR = "#00FF00";
    public const string NEUTRAL_COLOR = "#FFFFFF";
    public const string NEGATIVE_COLOR = "#FF0000";

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

        ModifierImpact impact = GetImpact();

        string color;
        switch(impact) {
            case ModifierImpact.POSITIVE:
                color = POSITIVE_COLOR;
                break;
            case ModifierImpact.NEUTRAL:
                color = NEUTRAL_COLOR;
                break;
            case ModifierImpact.NEGATIVE:
                color = NEGATIVE_COLOR;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return $"<color={color}>{valueString}</color> {StatType.GetDisplayName()}";
    }

    public ModifierImpact GetImpact() {
        switch(StatType) {
            case StatType.MAX_HEALTH:
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