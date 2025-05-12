using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LivingEntity))]
[RequireComponent(typeof(BehaviorGraphAgent))]
public class MushroomAIController : MonoBehaviour {
    public WeaponHolder WeaponHolder;
    public WeaponItemData PrimaryAttackWeapon;
    public Effect StunEffect;
    public VisualEffect PullVFX;

    private LivingEntity _livingEntity;
    private Animator _animator;
    private BehaviorGraphAgent _behaviorGraphAgent;
    private MushroomPullComponent _pullTarget;

    private static readonly int _hurtHash = Animator.StringToHash("hurt");

    void Start() {
        _livingEntity = GetComponent<LivingEntity>();
        _animator = GetComponent<Animator>();
        _behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();

        _livingEntity.OnDamageTaken.AddListener(_ => {
            _animator.SetTrigger(_hurtHash);
        });
 
        WeaponHolder.UpdateWeapon(PrimaryAttackWeapon);
    }

    public void OnPrimaryAttackAnimationStart() {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.MushAttackStart, this.transform.position);
        WeaponHolder.InitializeAttack(AttackType.LIGHT);
        WeaponHolder.BeginAttack();
        WeaponHolder.EnableHitbox();
        AudioClip mainAttack = Resources.Load("SFX/grzyb/uderzenie") as AudioClip;
        // SoundFXManager.Instance.PlaySoundFXClip(mainAttack, transform, 1f);
    }

    public void OnPrimaryAttackAnimationEnd() {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.MushAttack, this.transform.position);
        WeaponHolder.DisableHitbox();
        WeaponHolder.EndAttack();
    }

    public void OnSecondaryAttackAnimationStart() {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.MushAttackStart, this.transform.position);
        if(!_behaviorGraphAgent.GetVariable("Target", out BlackboardVariable<LivingEntity> blackboardTarget)) {
            Debug.LogWarning($"Failed to get target from behavior graph agent");
            return;
        }

        LivingEntity target = blackboardTarget.Value;
        if(target == null) {
            Debug.LogWarning($"Target is null");
            return;
        }

        PullVFX.Play();

        _pullTarget = target.gameObject.AddComponent<MushroomPullComponent>();
        _pullTarget.Puller = gameObject;
        _pullTarget.OnPulled.AddListener(() => {
            target.ApplyEffect(StunEffect);
        });
    }

    public void OnSecondaryAttackAnimationEnd() {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.MushAttack, this.transform.position);
        PullVFX.Stop();
                
        if(_pullTarget == null) return;
       
        Destroy(_pullTarget);
    }
}
