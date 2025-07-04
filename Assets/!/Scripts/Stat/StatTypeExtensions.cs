using UnityEngine;

public static class StatTypeExtensions {
    public static string GetDisplayName(this StatType statType) {
        if (TextData.CurrentLanguage == "en") {
            switch (statType) {
                case StatType.MAX_HEALTH:
                    return "Max Health";
                case StatType.ARMOR:
                    return "Armor";
                case StatType.MOVEMENT_SPEED:
                    return "Movement Speed";
                case StatType.STRENGTH:
                    return "Strength";
                case StatType.VEKTHAR_CONTROL:
                    return "Vekhtar Control";
                case StatType.LIGHT_ATTACK_DAMAGE:
                    return "Light Attack Damage";
                case StatType.HEAVY_ATTACK_DAMAGE:
                    return "Heavy Attack Damage";
                default:
                    Debug.LogError($"Requested display name for stat type {statType} but it is not implemented, returning {statType}");
                    return statType.ToString();
            }
        } else {
            switch (statType) {
                case StatType.MAX_HEALTH:
                    return "Maksymalne Zdrowie";
                case StatType.ARMOR:
                    return "Pancerz";
                case StatType.MOVEMENT_SPEED:
                    return "Szybkość Ruchu";
                case StatType.STRENGTH:
                    return "Siła";
                case StatType.VEKTHAR_CONTROL:
                    return "Kontrola Vekthara";
                case StatType.LIGHT_ATTACK_DAMAGE:
                    return "Obrażenia Lekkiego Ataku";
                case StatType.HEAVY_ATTACK_DAMAGE:
                    return "Obrażenia Ciężkiego Ataku";
                default:
                    Debug.LogError($"Requested display name for stat type {statType} but it is not implemented, returning {statType}");
                    return statType.ToString();
            }
        }
    }
}