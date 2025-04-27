using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

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

    [Header("VFX")]
    public VisualEffect MorphVFX;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private LivingEntity _livingEntity;

    private static readonly int _isRealHash = Animator.StringToHash("isReal");
    private static readonly int _speedHash = Animator.StringToHash("speed");

    void Awake() {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _livingEntity = GetComponent<LivingEntity>();

        RealEnt.SetActive(false);
        FakeEnt.SetActive(true);

        LeftWeaponHolder.UpdateWeapon(PrimaryWeapon);
        RightWeaponHolder.UpdateWeapon(PrimaryWeapon);

        _livingEntity.IsInvisible = true;
    }

    public void MorphToReal() {
        MorphVFX.Play();

        _animator.SetBool(_isRealHash, true);
        _animator.SetFloat(_speedHash, 0);

        if(_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.ResetPath();
        }

        _livingEntity.IsInvisible = false;
    }

    public void MorphToFake() {
        MorphVFX.Play();

        _animator.SetBool(_isRealHash, false);
        _animator.SetFloat(_speedHash, 0);
    
        if(_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.ResetPath();
        }

        _livingEntity.IsInvisible = true;
    }

    public void OnRightHandAttackStart() {
        AudioClip attackSound = Resources.Load("SFX/drzewo/atak") as AudioClip; //hitsound
        SoundFXManager.Instance.PlaySoundFXClip(attackSound, transform, 1f);
        RightWeaponHolder.InitializeAttack(AttackType.LIGHT);
        RightWeaponHolder.BeginAttack();
        RightWeaponHolder.EnableHitbox();
    }

    public void OnRightHandAttackEnd() {
        RightWeaponHolder.EndAttack();
        RightWeaponHolder.DisableHitbox();
    }

    public void OnLeftHandAttackStart() {
        AudioClip attackSound = Resources.Load("SFX/drzewo/atak") as AudioClip; //hitsound
        SoundFXManager.Instance.PlaySoundFXClip(attackSound, transform, 1f);
        LeftWeaponHolder.InitializeAttack(AttackType.LIGHT);
        LeftWeaponHolder.BeginAttack();
        LeftWeaponHolder.EnableHitbox();
    }

    public void OnLeftHandAttackEnd() {
        LeftWeaponHolder.EndAttack();
        LeftWeaponHolder.DisableHitbox();
    }

    public void OnAppearStarted() {
        RealEnt.SetActive(true);
    }

    public void OnAppearEnded() {
        FakeEnt.SetActive(false);
        MorphVFX.Stop();
    }

    public void OnDisappearEnded() {
        MorphVFX.Stop();
        MorphVFX.SendEvent("StopOrbs");

        RealEnt.SetActive(false);
        FakeEnt.SetActive(true);

        if(_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.ResetPath();
        }
    }

    public void OnEntireAppearEnded() {
        MorphVFX.SendEvent("StopOrbs");
    }
}