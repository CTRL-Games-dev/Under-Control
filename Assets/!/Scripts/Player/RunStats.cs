using System.Collections.Generic;
using UnityEngine;

public enum RunStat {
    MaxHealth,
    Health,
    MaxMana,
    Mana,
    Armor,
    LightAttackDamage,
    LightAttackSpeed,
    LightAttackrange,
    HeavyAttackDamage,
    HeavyAttackSpeed,
    HeavyAttackRange,
    MovementSpeed,
    DashSpeed,
    DashCooldown,
    DashDistance,
}

public class RunStats : MonoBehaviour
{
    public float MaxHealth = 100;
    public float Health = 100;
    public float MaxMana = 100;
    public float Mana = 100;
    public float Armor = 0;
    public float LightAttackDamage = 10;
    public float LightAttackSpeed = 1;
    public float LightAttackRange = 1;
    public float HeavyAttackDamage = 20;
    public float HeavyAttackSpeed = 1;
    public float HeavyAttackRange = 1;
    public float MovementSpeed = 5;
    public float DashSpeed = 10;
    public float DashCooldown = 5;
    public float DashDistance = 5;



    public void SetStat(RunStat stat, float value) {
        setStatValue(stat, value);
    }

    public void AddToStat(RunStat stat, float value) {
        setStatValue(stat, getStatValue(stat) + value);
    }

    public void AddRunModifier(RunModifier runModifier) {
        AddToStat(runModifier.Modifier, runModifier.ModifierValue);
    }

    private void setStatValue(RunStat stat, float value) {
        switch(stat) {
            case RunStat.MaxHealth:
                MaxHealth = value;
                return;
            case RunStat.Health:
                Health = value;
                return;
            case RunStat.MaxMana:
                MaxMana = value;
                return;
            case RunStat.Mana:
                Mana = value;
                return;
            case RunStat.Armor:
                Armor = value;
                return;
            case RunStat.LightAttackDamage:
                LightAttackDamage = value;
                return;
            case RunStat.LightAttackSpeed:
                LightAttackSpeed = value;
                return;
            case RunStat.LightAttackrange:
                LightAttackRange = value;
                return;
            case RunStat.HeavyAttackDamage:
                HeavyAttackDamage = value;
                return;
            case RunStat.HeavyAttackSpeed:
                HeavyAttackSpeed = value;
                return;
            case RunStat.HeavyAttackRange:
                HeavyAttackRange = value;
                return;
            case RunStat.MovementSpeed:
                MovementSpeed = value;
                return;
            case RunStat.DashSpeed:
                DashSpeed = value;
                return;
            case RunStat.DashCooldown:
                DashCooldown = value;
                return;
            case RunStat.DashDistance:
                DashDistance = value;
                return;
        }
    }


    private float getStatValue(RunStat stat) {
        switch(stat) {
            case RunStat.MaxHealth:
                return MaxHealth;
            case RunStat.Health:
                return Health;
            case RunStat.MaxMana:
                return MaxMana;
            case RunStat.Mana:
                return Mana;
            case RunStat.Armor:
                return Armor;
            case RunStat.LightAttackDamage:
                return LightAttackDamage;
            case RunStat.LightAttackSpeed:
                return LightAttackSpeed;
            case RunStat.LightAttackrange:
                return LightAttackRange;
            case RunStat.HeavyAttackDamage:
                return HeavyAttackDamage;
            case RunStat.HeavyAttackSpeed:
                return HeavyAttackSpeed;
            case RunStat.HeavyAttackRange:
                return HeavyAttackRange;
            case RunStat.MovementSpeed:
                return MovementSpeed;
            case RunStat.DashSpeed:
                return DashSpeed;
            case RunStat.DashCooldown:
                return DashCooldown;
            case RunStat.DashDistance:
                return DashDistance;
            default:
                return 0;
        }
    }
}
