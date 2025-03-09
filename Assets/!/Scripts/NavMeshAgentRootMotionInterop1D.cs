using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

/*
    NavMeshAgent i animator domyslnie nie gadaja ze soba.
    Jest to interop ktory zapewnia te funkcjonalnosc.
    Wspiera tylko 1D - predkosc bez kierunku.
*/

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NavMeshAgentRootMotionInterop1D : MonoBehaviour {
    private readonly int _speedHash = Animator.StringToHash("speed");
    
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private float _speed = 0;

    void Start() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;
    }

    void Update() {
        Vector3 deltaPosition = _navMeshAgent.nextPosition - transform.position;
        deltaPosition.y = 0;

        _speed = Mathf.MoveTowards(_speed, deltaPosition.magnitude / Time.deltaTime, 0.5f);

        _animator.SetFloat(_speedHash, _speed);
    }

    public void OnAnimatorMove() {
        Vector3 rootPosition = _animator.rootPosition;
        rootPosition.y = _navMeshAgent.nextPosition.y;
        transform.position = rootPosition;
        _navMeshAgent.nextPosition = rootPosition;
    }
}
