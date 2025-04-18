using UnityEngine;

public static class StatTypeExtensions {
    public static string GetDisplayName(this StatType statType) {
        switch (statType) {
            case StatType.MAX_HEALTH:
                return "Max Health";
            case StatType.ARMOR:
                return "Armor";
            case StatType.ELEMENTAL_ARMOR:
                return "Elemental Armor";
            case StatType.MOVEMENT_SPEED:
                return "Movement Speed";
            case StatType.STRENGTH:
                return "Strength";
            case StatType.INTELLIGENCE:
                return "Intelligence";
            case StatType.PHYSICAL_ATTACK_DAMAGE:
                return "Physical Attack Damage";
            case StatType.ELEMENTAL_ATTACK_DAMAGE:
                return "Elemental Attack Damage";
            case StatType.VEKTHAR_CONTROL:
                return "Vekhtar Control";
            default:
                Debug.LogError($"Requested display name for stat type {statType} but it is not implemented, returning {statType}");
                return statType.ToString();
        }
    }
}