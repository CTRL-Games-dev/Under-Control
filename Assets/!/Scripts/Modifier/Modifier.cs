using System;

[Serializable]
public struct Modifier {
    public StatType statType;
    public float value;
    public ModifierType type;
    public Modifier(StatType statType, float value, ModifierType type = ModifierType.ADDITIVE) {
        this.statType = statType;
        this.value = value;
        this.type = type;
    }

    public float Calculate(float baseValue) {
        switch(type) {
            case ModifierType.ADDITIVE:
                return baseValue + value;
            case ModifierType.MULTIPLICATIVE:
                return baseValue * value;
            case ModifierType.PERCENT_CHANGE:
                //  20 => value * 1.2
                // -20 => value * 0.8
                return baseValue * (1 + value / 100);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override string ToString() {
        string valueString;
        switch(type) {
            case ModifierType.ADDITIVE:
                if (value > 0) {
                    valueString = $"+{value}";
                } else {
                    valueString = $"{value}";
                }

                break;
            case ModifierType.MULTIPLICATIVE:
                valueString = $"{value}x";
                break;
            case ModifierType.PERCENT_CHANGE:
                if (value > 0) {
                    valueString = $"+{value}%";
                } else {
                    valueString = $"{value}%";
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return $"{valueString} {statType.GetDisplayName()}";
    }

    public ModifierImpact GetImpact() {
        switch(statType) {
            case StatType.HEALTH:
                if (value > 0) {
                    return ModifierImpact.POSITIVE;
                } else if (value < 0) {
                    return ModifierImpact.NEGATIVE;
                } else {
                    return ModifierImpact.NEUTRAL;
                }
            case StatType.MAX_HEALTH:
                if (value > 0) {
                    return ModifierImpact.POSITIVE;
                } else if (value < 0) {
                    return ModifierImpact.NEGATIVE;
                } else {
                    return ModifierImpact.NEUTRAL;
                }
            case StatType.REGEN_RATE:
                if (value > 0) {
                    return ModifierImpact.POSITIVE;
                } else if (value < 0) {
                    return ModifierImpact.NEGATIVE;
                } else {
                    return ModifierImpact.NEUTRAL;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}