using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait For Enemy In Range", story: "[Agent] waits for enemy in range [Range] and stores it in [Variable]", category: "Action/LivingEntity", id: "af01083cd857dc066982e76e2cbbe472")]
public partial class WaitForEnemyInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<LivingEntity> Variable;

    private LivingEntity _livingEntity;

    protected override Status OnStart() {
        if(Agent.Value == null) {
            return Status.Failure;
        }

        if(!Agent.Value.TryGetComponent(out _livingEntity)) {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate() {
        Collider[] colliders = Physics.OverlapSphere(_livingEntity.transform.position, Range.Value);
        foreach (Collider collider in colliders) {
            if(!collider.TryGetComponent(out LivingEntity otherEntity)) {
                continue;
            }

            if(otherEntity.Guild.IsHostileTowards(_livingEntity.Guild)) {
                Variable.Value = otherEntity;
                return Status.Success;
            }
        }

        return Status.Running;
    }
}

