using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get Closest Enemy", story: "Get closest enemy to [Agent] and store it in [Target]", category: "Action/LivingEntity", id: "5b311c6235d0a9302f2d01e43ea3b67b")]
public partial class GetClosestEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<LivingEntity> Agent;
    [SerializeReference] public BlackboardVariable<LivingEntity> Target;

    protected override Status OnStart() {
        LivingEntity closestEnemy = null;
        Collider[] colliders = Physics.OverlapSphere(Agent.Value.transform.position, 1000);
        foreach (Collider collider in colliders) {
            if(!collider.TryGetComponent(out LivingEntity otherEntity)) {
                continue;
            }

            if(otherEntity.Guild.IsHostileTowards(Agent.Value.Guild)) {
                if(closestEnemy == null) {
                    closestEnemy = otherEntity;
                    continue;
                }

                if(Vector3.Distance(closestEnemy.transform.position, otherEntity.transform.position) > Vector3.Distance(closestEnemy.transform.position, Agent.Value.transform.position)) {
                    closestEnemy = otherEntity;
                }
            }
        }

        Target.Value = closestEnemy;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

