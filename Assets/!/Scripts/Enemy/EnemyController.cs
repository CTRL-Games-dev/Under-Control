using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : LivingEntity
{
    public Cooldown attackCooldown;
    public float attackRange = 1f;
    public float attackDamage = 1f;
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
        // Health
        health += regenRate * Time.deltaTime;
        if(health > maxHealth) {
            health = maxHealth;
        }

        if (target != null) {
            // Attack
            if(Vector3.Distance(target.transform.position, transform.position) < attackRange && attackCooldown.Execute()) {
                target.TakeDamage(attackDamage);
            }

            navMeshAgent.destination = target.transform.position;
        }

        // Animation
        animator.SetFloat(_animationIdVelocitySide, navMeshAgent.velocity.x);
        animator.SetFloat(_animationIdVelocityFront, navMeshAgent.velocity.z);
    }

    void OnDeath() {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }
}
