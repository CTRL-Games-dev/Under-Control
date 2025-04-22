using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait For Enemy Attack", story: "[Agent] waits for damage taken by enemy attack and stores enemy in [Variable]", category: "Action/LivingEntity")]
public partial class WaitForEnemyAttackAction : Action
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

        _livingEntity.OnDamageTaken.AddListener((DamageTakenEventData eventData) => {
            if(eventData.Attacker == null) return;

            CurrentStatus = Status.Success;
        });

        return Status.Running;
    }
}

