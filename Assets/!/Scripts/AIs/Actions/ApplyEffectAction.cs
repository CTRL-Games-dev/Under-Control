using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Apply Effect", story: "Applies [Effect] to [Target]", category: "Action/LivingEntity", id: "635d6f9975f583763c1e4221b44d33e3")]
public partial class ApplyEffectAction : Action
{
    [SerializeReference] public BlackboardVariable<Effect> Effect;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        if(Target.Value == null) {
            return Status.Failure;
        }

        if(Effect.Value == null) {
            return Status.Failure;
        }

        if(!Target.Value.TryGetComponent(out LivingEntity livingEntity)) {
            return Status.Failure;
        }

        livingEntity.ApplyEffect(Effect.Value);

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

