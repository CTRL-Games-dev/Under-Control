using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LivingEntity))]
public class SlimeAIController : MonoBehaviour {
    public const float JumpMaxHeight = 0.9f;
    public const float JumpMinHeight = 0.1f;
    public const float JumpSpeed = 2.8f;
    public const float JumpSlope = 1; // more = more aggresive / higher jump
    public const float MaxJumpDistance = 2.5f;
    public const float PreferredJumpDistance = 2f;

    public WeaponHolder WeaponHolder;
    public WeaponItemData WeaponItemData;
    public Cooldown JumpCooldown = new Cooldown(0.3f);
    public float AngularSpeed = 200;
    public bool IsJumping => _startingPoint.HasValue;

    public float CurrentJumpMaxDistance = MaxJumpDistance;
    public float CurrentPreferredJumpDistance = PreferredJumpDistance;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private LivingEntity _livingEntity;

    // Jump parameters
    private Vector3? _startingPoint;
    private Vector3? _destinationPoint;
    private bool _landingTriggered = false;

    private static readonly int _jumpAnimationHash = Animator.StringToHash("jump");
    private static readonly int _landingAnimationHash = Animator.StringToHash("landing");

    void Start() {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _livingEntity = GetComponent<LivingEntity>();

        _navMeshAgent.updatePosition = false;

        WeaponHolder.UpdateWeapon(WeaponItemData);
    }

    void Update() {
        // for(int i = 1; i < _navMeshAgent.path.corners.Length; i++) {
        //     Debug.DrawLine(_navMeshAgent.path.corners[i - 1], _navMeshAgent.path.corners[i], Color.red);
        // }

        if (IsJumping) {
            _navMeshAgent.nextPosition = _destinationPoint.Value;
            jumpUpdate();
            return;
        }

        _navMeshAgent.nextPosition = transform.position;

        if(_navMeshAgent.path.corners.Length < 2) {
            return;
        }

        List<Vector3> corners = _navMeshAgent.path.corners.ToList();
        float accountedDistance = 0;
        for(int i = corners.Count - 1; i > 0; i--) {
            float distance = Vector3.Distance(corners[i], corners[i - 1]);
            if (distance > _navMeshAgent.stoppingDistance - accountedDistance) {
                corners[i] = corners[i] - (corners[i] - corners[i - 1]).normalized * (_navMeshAgent.stoppingDistance - accountedDistance);
                break;
            } else {
                accountedDistance += distance;
                corners.RemoveAt(i);
            }
        }

        if(corners.Count == 1) return;

        Vector3 closestCorner = corners[1];
        Vector3 cornerDirection = closestCorner - transform.position;
        float cornerDistance = cornerDirection.magnitude;
        cornerDirection /= cornerDistance;

        CurrentJumpMaxDistance = MaxJumpDistance * _livingEntity.MovementSpeed;
        CurrentPreferredJumpDistance = PreferredJumpDistance * _livingEntity.MovementSpeed;

        Vector3 jumpDestination = closestCorner;
        if (cornerDistance > 2 * CurrentPreferredJumpDistance) {
            jumpDestination = transform.position + cornerDirection * CurrentPreferredJumpDistance;
        } else if (cornerDistance > CurrentJumpMaxDistance) {
            jumpDestination = transform.position + cornerDirection * cornerDistance / 2;
        } else if (cornerDistance > CurrentPreferredJumpDistance) {
            jumpDestination = transform.position + cornerDirection * cornerDistance;
        }

        Jump(jumpDestination);
    }

    public void Jump(Vector3 destination) {
        if(!JumpCooldown.Execute()) {
            return;
        }

        _startingPoint = transform.position;
        _destinationPoint = destination;

        _animator.SetTrigger(_jumpAnimationHash);

        AudioManager.instance.PlayOneShot(FMODEvents.instance.SlimeJump, transform.position);



        // Debug.Log($"Jumping to {destination} from {transform.position} | Distance: {Vector3.Distance(transform.position, destination)}");
    }

    private void jumpUpdate() {
        float simpleJumpLength = Vector3.Distance(_startingPoint.Value, _destinationPoint.Value);

        // Punish small jump's speed
        float jumpSpeed = JumpSpeed * simpleJumpLength /  CurrentJumpMaxDistance;

        float totalDistX = _destinationPoint.Value.x - _startingPoint.Value.x;
        float totalDistZ = _destinationPoint.Value.z - _startingPoint.Value.z;
        float totalDistXZ = Mathf.Sqrt(totalDistX * totalDistX + totalDistZ * totalDistZ);

        float distNextX = _destinationPoint.Value.x - transform.position.x;
        float distNextZ = _destinationPoint.Value.z - transform.position.z;
        float distNextXZ = Mathf.Sqrt(distNextX * distNextX + distNextZ * distNextZ);

        float progress = 1 - distNextXZ / totalDistXZ;

        Vector2 nextPosFlat = Vector2.MoveTowards(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(_destinationPoint.Value.x, _destinationPoint.Value.z),
            jumpSpeed * Time.deltaTime
        );

        // Arc
        // doslownie robilem rownanie kwadratowe do tego

        float a = -JumpSlope;
        float height = -totalDistXZ * totalDistXZ * a / 4;
        if(height > JumpMaxHeight) {
            a = -4 * JumpMaxHeight / (totalDistXZ * totalDistXZ);
        } else if(height < JumpMinHeight) {
            a = -4 * JumpMinHeight / (totalDistXZ * totalDistXZ);
        }

        float arc = a * (totalDistXZ - distNextXZ) * -distNextXZ;

        float baseY = Mathf.Lerp(_startingPoint.Value.y, _destinationPoint.Value.y, progress);

        if(float.IsNaN(baseY + arc)) {
            Debug.LogWarning($"baseY + arc is NaN. baseY: {baseY} | arc: {arc}");
            Debug.LogWarning($"totalDistXZ: {totalDistXZ} | distNextXZ: {distNextXZ}");
            Debug.LogWarning($"progress: {progress}");
            Debug.LogWarning($"a: {a}");
            Debug.LogWarning($"height: {height}");
            Debug.LogWarning($"JumpMaxHeight: {JumpMaxHeight}");
            Debug.LogWarning($"JumpMinHeight: {JumpMinHeight}");
            Debug.LogWarning($"_startingPoint: {_startingPoint.Value}");
            Debug.LogWarning($"_destinationPoint: {_destinationPoint.Value}");
            Debug.LogError("BLAD! ZGLOS LONDONOWI BO NIE MIAL REPRO");
            Debug.Break();
        }

        //
        // Debug.Log($"Jump speed: {jumpSpeed} | Arc: {arc} | BaseY: {baseY} | Progress: {progress}");

        transform.position = new Vector3(nextPosFlat.x, baseY + arc, nextPosFlat.y);

        if(progress > 0.95 && !_landingTriggered) {
            _animator.SetTrigger(_landingAnimationHash);
            _landingTriggered = true;
        }

        if(progress == 1) {
            JumpCooldown.ForceExecute();

            _startingPoint = null;
            _destinationPoint = null;
            _landingTriggered = false;

            _navMeshAgent.ResetPath();
            _navMeshAgent.isStopped = true;

            return;
        }
    }

    public void OnAttackAnimationStart() {
        WeaponHolder.InitializeAttack(AttackType.LIGHT);
        WeaponHolder.BeginAttack();
        WeaponHolder.EnableHitbox();
    }

    public void OnAttackAnimationEnd() {
        WeaponHolder.DisableHitbox();
        WeaponHolder.EndAttack();
    }
}