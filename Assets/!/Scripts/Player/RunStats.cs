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

    private Dictionary<RunStat, float> _runStats;

    private void Awake() {
        _runStats = new Dictionary<RunStat, float>{
            {RunStat.MaxHealth, MaxHealth},
            {RunStat.Health, Health},
            {RunStat.MaxMana, MaxMana},
            {RunStat.Mana, Mana},
            {RunStat.Armor, Armor},
            {RunStat.LightAttackDamage, LightAttackDamage},
            {RunStat.LightAttackSpeed, LightAttackSpeed},
            {RunStat.LightAttackrange, LightAttackRange},
            {RunStat.HeavyAttackDamage, HeavyAttackDamage},
            {RunStat.HeavyAttackSpeed, HeavyAttackSpeed},
            {RunStat.HeavyAttackRange, HeavyAttackRange},
            {RunStat.MovementSpeed, MovementSpeed},
            {RunStat.DashSpeed, DashSpeed},
            {RunStat.DashCooldown, DashCooldown},
            {RunStat.DashDistance, DashDistance},
        };
    }

    public void SetStat(RunStat stat, float value) {
        _runStats[stat] = value;
    }

    public void AddToStat(RunStat stat, float value) {
        _runStats[stat] += value;
    }

    public float GetStat(RunStat stat) {
        return _runStats[stat];
    }
}
