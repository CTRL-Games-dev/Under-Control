// cc: Unity => NavigateToLocationAction

using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Go To Random Spot In Radius", story: "[Agent] goes to random spot in radius [Radius]", category: "Action/Fixed Navigation", id: "93b1e61da4728c05a0a1887b06ceb284")]
public partial class GoToRandomSpotInRadiusAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Radius = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> StoppingDistance = new BlackboardVariable<float>(0.2f);

    private float _previousStoppingDistance;
    private NavMeshAgent _navMeshAgent;

    protected override Status OnStart() {
        if (Agent.Value == null || Radius.Value == 0) {
            return Status.Failure;
        }

        return Initialize();
    }

    protected override Status OnUpdate() {
        if (Agent.Value == null || Radius.Value == 0) {
            return Status.Failure;
        }

        if (_navMeshAgent.remainingDistance <= StoppingDistance.Value) {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd() {
        if (_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.ResetPath();
        }

        _navMeshAgent.stoppingDistance = _previousStoppingDistance;

        _navMeshAgent = null;
    }

    protected override void OnDeserialize() {
        Initialize();
    }

    private Status Initialize() {
        if(!Agent.Value.TryGetComponent(out _navMeshAgent)) {
            return Status.Failure;
        }
        
        if(!NavMesh.SamplePosition(
            Agent.Value.transform.position + getRandomOffset(),
            out NavMeshHit hit,
            Radius.Value,
            NavMesh.AllAreas
        )) {
            return Status.Failure;
        }

        if(!hit.hit) {
            return Status.Failure;
        }

        Vector3 locationPosition = hit.position;

        if (GetDistanceToLocation(locationPosition) <= StoppingDistance) {
            return Status.Success;
        }

        if (_navMeshAgent.isOnNavMesh) {
            _navMeshAgent.ResetPath();
        }

        _navMeshAgent.speed = Speed;
        _previousStoppingDistance = _navMeshAgent.stoppingDistance;
        _navMeshAgent.stoppingDistance = StoppingDistance;
        _navMeshAgent.SetDestination(locationPosition);

        return Status.Running;
    }

    private float GetDistanceToLocation(Vector3 locationPosition) {
        Vector3 agentPosition = Agent.Value.transform.position;
        return Vector3.Distance(new Vector3(agentPosition.x, locationPosition.y, agentPosition.z), locationPosition);
    }

    private Vector3 getRandomOffset() {
        float radius = Radius.Value;

        return new Vector3(
            UnityEngine.Random.Range(-radius, radius),
            UnityEngine.Random.Range(-radius, radius),
            UnityEngine.Random.Range(-radius, radius)
        );
    }
}

