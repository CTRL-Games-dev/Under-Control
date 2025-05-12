using System;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public abstract class PlayerWeaponAttack : MonoBehaviour
{
    [Range(0, 1)] public float ApplyChance = 0.5f;
    public Color TintColor = Color.white;
    public float TintAlpha = 0f;
    public float Duration = 2f;

    public float ManaCost = 0f;

    public abstract void Attack(LivingEntity victim);
}