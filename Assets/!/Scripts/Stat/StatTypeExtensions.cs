using System;

public static class StatTypeExtensions {
    public static string GetDisplayName(this StatType statType) {
        switch (statType) {
            case StatType.HEALTH:
                return "Health";
            case StatType.MAX_HEALTH:
                return "Max Health";
            case StatType.REGEN_RATE:
                return "Regen Rate";
            case StatType.ARMOR:
                return "Armor";
            case StatType.ELEMENTAL_ARMOR:
                return "Elemental Armor";
            case StatType.MOVEMENT_SPEED:
                return "Movement Speed";
            case StatType.STENGTH:
                return "Strength";
            case StatType.INTELLIGENCE:
                return "Intelligence";
            case StatType.PHYSICAL_ATTACK_DAMAGE:
                return "Physical Attack Damage";
            case StatType.ELEMENTAL_ATTACK_DAMAGE:
                return "Elemental Attack Damage";
            case StatType.VEKTHAR_CONTROL:
                return "Vekhtar Control";
            case StatType.EXP_DROPPED:
                return "Exp Dropped";
            default:
                throw new ArgumentOutOfRangeException(nameof(statType), statType, null);
        }
    }
}