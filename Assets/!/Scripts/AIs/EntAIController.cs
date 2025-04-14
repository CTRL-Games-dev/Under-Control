using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LivingEntity))]
[RequireComponent(typeof(BehaviorGraphAgent))]
public class EntAIController : MonoBehaviour {
    [Header("Weapon")]
    public WeaponHolder LeftWeaponHolder;
    public WeaponHolder RightWeaponHolder;
    public WeaponItemData PrimaryWeapon;

    [Header("Ents")]
    public GameObject RealEnt;
    public GameObject FakeEnt;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    private static readonly int _isRealHash = Animator.StringToHash("isReal");
    private static readonly int _speedHash = Animator.StringToHash("speed");

    void Awake() {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        RealEnt.SetActive(false);
        FakeEnt.SetActive(true);

        LeftWeaponHolder.UpdateWeapon(PrimaryWeapon);
        RightWeaponHolder.UpdateWeapon(PrimaryWeapon);
    }

    public void MorphToReal() {
        _animator.SetBool(_isRealHash, true);
        _animator.SetFloat(_speedHash, 0);

        RealEnt.SetActive(true);
        FakeEnt.SetActive(false);
 
        if(_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.ResetPath();
        }
    }

    public void MorphToFake() {
        _animator.SetBool(_isRealHash, false);
        _animator.SetFloat(_speedHash, 0);
      
        RealEnt.SetActive(false);
        FakeEnt.SetActive(true);

        if(_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.ResetPath();
        }
    }

    public void OnAttackAnimationStart() {
        LeftWeaponHolder.InitializeAttack(AttackType.LIGHT);
        LeftWeaponHolder.BeginAttack();
        LeftWeaponHolder.EnableHitbox();

        RightWeaponHolder.InitializeAttack(AttackType.LIGHT);
        RightWeaponHolder.BeginAttack();
        RightWeaponHolder.EnableHitbox();
    }

    public void OnAttackAnimationEnd() {
        LeftWeaponHolder.EndAttack();
        LeftWeaponHolder.DisableHitbox();

        RightWeaponHolder.EndAttack();
        RightWeaponHolder.DisableHitbox();
    }
}