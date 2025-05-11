using UnityEngine;
using UnityEngine.AI;

/*
    NavMeshAgent i Animator domyslnie nie gadaja ze soba.
    Jest to interop ktory zapewnia te funkcjonalnosc.
    Wspiera tylko 1D - predkosc bez kierunku.

    Zawiera fixy do pozycji i rotacji nie związane z root motion.
    Doslownie mechaniki w NavMeshAgencie sa zwalone i jest o tym
    goraca dyskusja na forach.
*/

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NavMeshAgentRootMotionInterop1D : MonoBehaviour {
    public enum MotionCoherenceStrategy {
        None,
        KeepWithinRange,
        Teleport,
        EarlyAvoid,
        // ComeBack,
    }

    private static readonly int _speedHash = Animator.StringToHash("speed");
    
    [Range(0, 1)]
    public float SpeedSmoothingLength = 0.15f;

    public MotionCoherenceStrategy CoherenceStrategy;
    public float MaxPositionError = 1;
    public bool FollowAgentY = true;
    public bool UseAgentAcceleration = true; 
    public bool DecelerationIsAcceleration = false;
    public float Acceleration => UseAgentAcceleration ? _navMeshAgent.acceleration : _acceleration;
    public float Deceleration => DecelerationIsAcceleration ? Acceleration : _deceleration;
    public AnimationCurve AngularSpeedMultiplierCurve = AnimationCurve.Constant(0, 1, 1);

    public float MovementSpeedMultiplier { get => _livingEntity != null ? _livingEntity.MovementSpeed : 1; }

    [SerializeField]
    private float _deceleration = 4f;
    [SerializeField]
    private float _acceleration = 2f;

    private NavMeshAgent _navMeshAgent;
    private LivingEntity _livingEntity;
    private Animator _animator;
    private float _speed = 0;
    private Vector3 _velocity = Vector3.zero;

    void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _livingEntity = GetComponent<LivingEntity>();

        _animator.applyRootMotion = true;

        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
    }

    void Update() {
        float maxMagnitudeDelta;
        if(_navMeshAgent.desiredVelocity.magnitude > _velocity.magnitude) {
            maxMagnitudeDelta = Acceleration * Time.deltaTime;
        } else {
            maxMagnitudeDelta = Deceleration * Time.deltaTime;
        }

        _velocity = Vector3.RotateTowards(
            _velocity,
            _navMeshAgent.desiredVelocity,
            _navMeshAgent.angularSpeed * Mathf.Deg2Rad * Time.deltaTime,
            maxMagnitudeDelta
        );

        if(SpeedSmoothingLength > 0) {
            _speed = Mathf.Lerp(_speed, _velocity.magnitude, Mathf.Min(1, Time.deltaTime/SpeedSmoothingLength));
        } else {
            _speed = _velocity.magnitude;
        }

        _animator.SetFloat(_speedHash, _speed);

        if(_velocity * MovementSpeedMultiplier != Vector3.zero) {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(_velocity),
                _navMeshAgent.angularSpeed * AngularSpeedMultiplierCurve.Evaluate(_speed) * Time.deltaTime
            );
        }

        HandleMotionCoherence();

        _navMeshAgent.velocity = _velocity * MovementSpeedMultiplier;
    }

    public void OnAnimatorMove() {
        Vector3 rootPosition = _animator.rootPosition;
      
        if(FollowAgentY) {
            rootPosition.y = _navMeshAgent.nextPosition.y;
        }

        transform.position = rootPosition;
        _navMeshAgent.nextPosition = rootPosition;
    }

    public void HandleMotionCoherence() {
        if(CoherenceStrategy == MotionCoherenceStrategy.None) {
            return;
        }

        if(CoherenceStrategy == MotionCoherenceStrategy.KeepWithinRange) {
            handleKeepWithinRangeMotionCoherence();
        }

        if(CoherenceStrategy == MotionCoherenceStrategy.Teleport) {
            handleTeleportMotionCoherence();
        }

        if(CoherenceStrategy == MotionCoherenceStrategy.EarlyAvoid) {
            handleEarlyAvoidMotionCoherence();
        }
    }

    private void handleKeepWithinRangeMotionCoherence() {
        Vector3 agentPosition = _navMeshAgent.nextPosition;
        Vector3 rootPosition = transform.position;

        float distance = Vector3.Distance(agentPosition, rootPosition);
        if(distance < MaxPositionError) {
            return;
        }

        transform.position = agentPosition + (rootPosition - agentPosition).normalized * MaxPositionError;
    }

    private void handleTeleportMotionCoherence() {
        Vector3 agentPosition = _navMeshAgent.nextPosition;
        Vector3 rootPosition = transform.position;
        
        float distance = Vector3.Distance(agentPosition, rootPosition);
        if(distance < MaxPositionError) {
            return;
        }
        
        transform.position = agentPosition;
    }

    private void handleEarlyAvoidMotionCoherence() {
        Vector3 agentPosition = _navMeshAgent.nextPosition;
        Vector3 rootPosition = transform.position;

        float distance = Vector3.Distance(agentPosition, rootPosition);
        if(distance < 0.1f) {
            // Not considered an error yet
            return;
        }

        Vector3 comebackDirection = agentPosition - rootPosition;

        Vector3 targetVelocity = _navMeshAgent.speed * comebackDirection.normalized;

        float maxMagnitudeDelta;
        if(targetVelocity.magnitude > _velocity.magnitude) {
            maxMagnitudeDelta = Acceleration * Time.deltaTime;
        } else {
            maxMagnitudeDelta = Deceleration * Time.deltaTime;
        }

        // transform.position = Vector3.RotateTowards(
        //     _velocity,
        //     targetVelocity,
        //     _navMeshAgent.angularSpeed * Mathf.Deg2Rad * Time.deltaTime,
        //     maxMagnitudeDelta
        // );

        // if(distance > MaxPositionError * 0.9) {
        //     // Time to press the brakes
        //     _velocity = Vector3.Lerp(_velocity, targetVelocity, 0.5f);
        // }

        // if(targetVelocity.magnitude > _velocity.magnitude) {
        //     maxMagnitudeDelta = Acceleration * Time.deltaTime;
        // } else {
        //     maxMagnitudeDelta = Deceleration * Time.deltaTime;
        // }

        transform.position = Vector3.RotateTowards(
            transform.position,
            agentPosition,
            _navMeshAgent.angularSpeed * Mathf.Deg2Rad * Time.deltaTime,
            maxMagnitudeDelta
        );
    }
}
