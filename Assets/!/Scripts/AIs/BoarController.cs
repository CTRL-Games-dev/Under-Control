using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LivingEntity))]
public class BoarController : MonoBehaviour 
{
    public WeaponHolder WeaponHolder;
    public WeaponItemData PrimaryAttackWeapon;
    public WeaponItemData ChargeAttackWeapon;

    private Animator _animator;

    void Start() {
        _animator = GetComponent<Animator>();
    }

    public void OnAttackAnimationStart() {
        WeaponHolder.UpdateWeapon(PrimaryAttackWeapon);
        WeaponHolder.InitializeAttack(AttackType.LIGHT);
        WeaponHolder.BeginAttack();
        WeaponHolder.EnableHitbox();
    }

    public void OnAttackAnimationEnd() {
        WeaponHolder.DisableHitbox();
        WeaponHolder.EndAttack();
    }

    public void OnChargeAnimationStart() {
        WeaponHolder.UpdateWeapon(ChargeAttackWeapon);
        WeaponHolder.InitializeAttack(AttackType.LIGHT);
        WeaponHolder.BeginAttack();
        WeaponHolder.EnableHitbox();
    }

    public void OnChargeAnimationEnd() {
        WeaponHolder.DisableHitbox();
        WeaponHolder.EndAttack();
    }
}
