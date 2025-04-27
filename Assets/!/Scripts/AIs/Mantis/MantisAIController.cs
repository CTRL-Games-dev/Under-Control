using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LivingEntity))]
[RequireComponent(typeof(BehaviorGraphAgent))]
public class MantisAIController : MonoBehaviour {
    [Header("Weapon")]
    public WeaponHolder WeaponHolder;
    public WeaponItemData PrimaryWeapon;

    void Awake() {
        WeaponHolder.UpdateWeapon(PrimaryWeapon);
    }

    public void OnAttackAnimationStart() {
        WeaponHolder.InitializeAttack(AttackType.LIGHT);
        WeaponHolder.BeginAttack();
        WeaponHolder.EnableHitbox();
    }

    public void OnAttackAnimationEnd() {
        WeaponHolder.EndAttack();
        WeaponHolder.DisableHitbox();
    }
}