using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Navigate To Target", story: "Fixed: [Agent] navigates to [Target]", category: "Action/Fixed Navigation", id: "8b44d9e971518c9c4cf954302e449bad")]
public partial class NavigateToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> StoppingDistance = new BlackboardVariable<float>(1.0f);

    private NavMeshAgent m_NavMeshAgent;
    private float m_PreviousStoppingDistance;
    private Vector3 m_LastTargetPosition;

    protected override Status OnStart()
    {
        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        // Check if the target position has changed.
        bool boolUpdateTargetPosition = !Mathf.Approximately(m_LastTargetPosition.x, Target.Value.transform.position.x) || !Mathf.Approximately(m_LastTargetPosition.y, Target.Value.transform.position.y) || !Mathf.Approximately(m_LastTargetPosition.z, Target.Value.transform.position.z);
        if (boolUpdateTargetPosition)
        {
            m_LastTargetPosition = Target.Value.transform.position;
        }

        float distance = GetDistanceXZ();
        if (distance <= StoppingDistance)
        {
            return Status.Success;
        }

        if (boolUpdateTargetPosition)
        {
            m_NavMeshAgent.SetDestination(Target.Value.transform.position);
        }

        if (m_NavMeshAgent.IsNavigationComplete())
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (m_NavMeshAgent.isOnNavMesh)
        {
            m_NavMeshAgent.ResetPath();
        }

        m_NavMeshAgent.stoppingDistance = m_PreviousStoppingDistance;
        m_NavMeshAgent = null;
    }

    protected override void OnDeserialize()
    {
        Initialize();
    }

    private Status Initialize()
    {
        if(!Agent.Value.TryGetComponent(out m_NavMeshAgent)) {
            return Status.Failure;
        }

        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }
        
        m_LastTargetPosition = Target.Value.transform.position;

        if (GetDistanceXZ() <= StoppingDistance)
        {
            return Status.Success;
        }
        
        if (m_NavMeshAgent.isOnNavMesh)
        {
            m_NavMeshAgent.ResetPath();
        }

        m_NavMeshAgent.speed = Speed;
        m_PreviousStoppingDistance = m_NavMeshAgent.stoppingDistance;

        m_NavMeshAgent.stoppingDistance = StoppingDistance;
        m_NavMeshAgent.SetDestination(Target.Value.transform.position);

        return Status.Running;
    }

    private float GetDistanceXZ() {
        Vector2 agentPosition = new Vector2(Agent.Value.transform.position.x, Agent.Value.transform.position.z);
        Vector2 targetPosition = new Vector2(Target.Value.transform.position.x, Target.Value.transform.position.z);
        return Vector2.Distance(agentPosition, targetPosition);
    }
}

