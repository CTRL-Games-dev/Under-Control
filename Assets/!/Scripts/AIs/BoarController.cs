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
    public Cooldown PrimaryAttackCooldown;

    private Animator _animator;

    private static readonly int _primaryAttackHash = Animator.StringToHash("primaryAttack");

    void Start() {
        _animator = GetComponent<Animator>();
    }

    public void PrimaryAttack() {
        if(!PrimaryAttackCooldown.Execute()) return;

        WeaponHolder.UpdateWeapon(PrimaryAttackWeapon);
        WeaponHolder.InitializeAttack(AttackType.LIGHT);
        _animator.SetTrigger(_primaryAttackHash);
    }

    public void ChargeAttack() {

    }

    public void OnAttackAnimationStart() {
        WeaponHolder.BeginAttack();
        WeaponHolder.EnableHitbox();
    }

    public void OnAttackAnimationEnd() {
        WeaponHolder.DisableHitbox();
        WeaponHolder.EndAttack();
    }
}
