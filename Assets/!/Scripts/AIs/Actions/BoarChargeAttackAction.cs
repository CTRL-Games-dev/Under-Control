using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boar Charge Attack", story: "[Agent] executes Charge Attack", category: "Action", id: "fe0f8a27a79d1e6981a782676dd16a12")]
public partial class BoarChargeAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private BoarController m_BoarController;

    protected override Status OnStart() {
        m_BoarController = Agent.Value.GetComponent<BoarController>();

        return Status.Running;
    }

    protected override Status OnUpdate() {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

