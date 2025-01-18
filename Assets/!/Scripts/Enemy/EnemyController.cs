using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LivingEntity))]
public class EnemyController : MonoBehaviour
{
    public Cooldown AttackCooldown;
    public float AttackRange = 1;
    public float AttackDamage = 1;
    public LivingEntity Target;

    // References
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    // Animation IDs
    private int _animationIdVelocitySide = Animator.StringToHash("velocitySide");
    private int _animationIdVelocityFront = Animator.StringToHash("velocityFront");

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Target != null) {
            // Attack
            if(Vector3.Distance(Target.transform.position, transform.position) < AttackRange && AttackCooldown.Execute()) {
                Target.TakeDamage(new Damage {
                    Type = DamageType.PHYSICAL,
                    Value = AttackDamage
                });
            }

            _navMeshAgent.destination = Target.transform.position;
        }

        // Animation
        _animator.SetFloat(_animationIdVelocitySide, _navMeshAgent.velocity.x);
        _animator.SetFloat(_animationIdVelocityFront, _navMeshAgent.velocity.z);
    }
}
