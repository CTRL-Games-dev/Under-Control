using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LivingEntity))]
public class SimpleEnemyController : MonoBehaviour
{
    [Header("Properties")]
    public Cooldown AttackCooldown;
    public float AttackRange = 1;
    public float AttackDamage = 1;
    public LivingEntity Target;

    // References
    private NavMeshAgent _navMeshAgent;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
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
    }
}
