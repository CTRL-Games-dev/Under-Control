using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LivingEntity))]
[RequireComponent(typeof(BehaviorGraphAgent))]
public class MushroomController : MonoBehaviour {
    public WeaponHolder WeaponHolder;
    public WeaponItemData PrimaryAttackWeapon;
    public Effect StunEffect;

    private BehaviorGraphAgent _behaviorGraphAgent;
    private MushroomPullComponent _pullTarget;

    void Start() {
        _behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
    }

    public void OnPrimaryAttackAnimationStart() {
        WeaponHolder.UpdateWeapon(PrimaryAttackWeapon);
        WeaponHolder.InitializeAttack(AttackType.LIGHT);
        WeaponHolder.BeginAttack();
        WeaponHolder.EnableHitbox();
    }

    public void OnPrimaryAttackAnimationEnd() {
        WeaponHolder.DisableHitbox();
        WeaponHolder.EndAttack();
    }

    public void OnSecondaryAttackAnimationStart() {
        if(!_behaviorGraphAgent.GetVariable("Target", out BlackboardVariable<LivingEntity> blackboardTarget)) {
            Debug.LogWarning($"Failed to get target from behavior graph agent");
            return;
        }

        LivingEntity target = blackboardTarget.Value;
        if(target == null) {
            Debug.LogWarning($"Target is null");
            return;
        }

        _pullTarget = target.gameObject.AddComponent<MushroomPullComponent>();
        _pullTarget.Puller = gameObject;
        _pullTarget.OnPulled.AddListener(() => {
            target.ApplyEffect(StunEffect);
        });
    }

    public void OnSecondaryAttackAnimationEnd() {
        if(_pullTarget == null) return;

        Destroy(_pullTarget);
    }
}
