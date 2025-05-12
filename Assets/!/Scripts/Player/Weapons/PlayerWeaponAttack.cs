using System;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public abstract class PlayerWeaponAttack : MonoBehaviour
{
    public Color TintColor = Color.white;
    public float TintAlpha = 0f;
    public float Duration = 2f;

    public float ManaCost = 0f;

    public abstract void Attack(LivingEntity victim);
}