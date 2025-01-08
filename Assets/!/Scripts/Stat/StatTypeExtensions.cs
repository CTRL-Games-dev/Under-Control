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
            default:
                throw new ArgumentOutOfRangeException(nameof(statType), statType, null);
        }
    }
}