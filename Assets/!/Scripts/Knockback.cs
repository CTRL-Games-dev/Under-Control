using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LivingEntity))]
public class Knockback : MonoBehaviour {
    public float Weight = 0.5f;
    public float Deceleration = 10f;
    public float MaxKnockback = 20f;

    private Vector3 _knockbackVector = Vector3.zero;
    private bool _animatorRootMotionEnabled = true;

    private NavMeshAgent _navMeshAgent;
    private LivingEntity _livingEntity;
    private Animator _animator;
    private NavMeshAgentRootMotionInterop1D _navMeshAgentRootMotionInterop1D;

    private Rigidbody _rigidbody;
    private CharacterController _characterController;

    void Awake() {
        _livingEntity = GetComponent<LivingEntity>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _navMeshAgentRootMotionInterop1D = GetComponent<NavMeshAgentRootMotionInterop1D>(); 
    
        _rigidbody = GetComponent<Rigidbody>();
        _characterController = GetComponent<CharacterController>();
    }

    void Start() {
        _livingEntity.OnDamageTaken.AddListener(OnDamageTaken);

        if (_animator != null) {
            _animatorRootMotionEnabled = _animator.applyRootMotion;
        }
    }

    void FixedUpdate() {
        _knockbackVector -= _knockbackVector * Time.fixedDeltaTime * Deceleration;

        if (_knockbackVector.magnitude < 0.2) {
            _knockbackVector = Vector3.zero;

            if (_animator != null) {
                _animator.applyRootMotion = _animatorRootMotionEnabled;
            }

            if (_navMeshAgentRootMotionInterop1D != null) {
                _navMeshAgentRootMotionInterop1D.enabled = true;
            }
        } else {
            if (_navMeshAgent != null) {
                _navMeshAgent.nextPosition = transform.position;
            }

            if (_animator != null) {
                _animator.applyRootMotion = false;
            }

            if (_navMeshAgentRootMotionInterop1D != null) {
                _navMeshAgentRootMotionInterop1D.enabled = false;
            }

            if (_characterController != null) {
                _characterController.Move(_knockbackVector * Time.fixedDeltaTime);
            } else {
                _rigidbody.MovePosition(_rigidbody.position + _knockbackVector * Time.fixedDeltaTime);
            }
        }
    }

    public void OnDamageTaken(DamageTakenEventData data) {
        if(data.Attacker == null) return;

        Vector3 direction = transform.position - data.Attacker.transform.position;

        _knockbackVector += direction.normalized * data.ActualDamageAmount / Weight;
        _knockbackVector.y = 0;
        _knockbackVector = Vector3.ClampMagnitude(_knockbackVector, MaxKnockback);
    }

    public void Reset() {
        _knockbackVector = Vector3.zero;
    }
}