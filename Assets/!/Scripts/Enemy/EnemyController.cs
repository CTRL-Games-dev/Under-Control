using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LivingEntity))]
public class EnemyController : MonoBehaviour
{
    public Cooldown attackCooldown;
    public float attackRange = 1;
    public float attackDamage = 1;
    public LivingEntity target;

    // References
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    // Animation IDs
    private int _animationIdVelocitySide = Animator.StringToHash("velocitySide");
    private int _animationIdVelocityFront = Animator.StringToHash("velocityFront");

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (target != null) {
            // Attack
            if(Vector3.Distance(target.transform.position, transform.position) < attackRange && attackCooldown.Execute()) {
                target.TakeDamage(new Damage {
                    type = DamageType.PHYSICAL,
                    value = attackDamage
                });
            }

            navMeshAgent.destination = target.transform.position;
        }

        // Animation
        animator.SetFloat(_animationIdVelocitySide, navMeshAgent.velocity.x);
        animator.SetFloat(_animationIdVelocityFront, navMeshAgent.velocity.z);
    }
}
