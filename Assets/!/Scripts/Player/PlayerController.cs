using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(LivingEntity))]
public class PlayerController : MonoBehaviour
{
    [Header("Properties")]
    public float Acceleration = 2f;
    public float Deceleration = 2f;
    public float MaxWalkingSpeed = 1f;
    public float MaxSprintingSpeed = 2f;
    public float MouseSensitivity = 0.1f;
    public float WalkingTurnSpeed = 1f;
    // public float SprintingTurnSpeed = 2.5f;
    public float MinCameraDistance = 10f;
    public float MaxCameraDistance = 30f;
    public float CameraDistanceSpeed = 1f;
    public Vector2 CameraTargetObjectBounds = Vector2.zero;
    public GameObject CameraObject;
    public GameObject CameraTargetObject;

    [Header("Stats")]
    public DynamicStat VekhtarControl = new DynamicStat(StatType.VEKTHAR_CONTROL, 0);
    private int _coins = 100;
    public int Coins { 
        get{
            return _coins;
        } set {
            CoinsChangeEvent?.Invoke(value - _coins);
            _coins = value;
        }  
    } 

    [Header("Events")]
    public UnityEvent InventoryToggleEvent;
    public UnityEvent UICancelEvent;
    public UnityEvent ItemRotateEvent;
    public UnityEvent<int> CoinsChangeEvent;
    // public UnityEvent 

    // State
    private Vector2 _movementInputVector = Vector2.zero;
    // private Vector3 _targetDirection;
    // private bool _sprinting = false;
    // private float _velocitySide = 0;
    // private float _velocityFront = 0;
    // private bool _isTurning;
    private float _cameraDistance { get => CinemachinePositionComposer.CameraDistance; set => CinemachinePositionComposer.CameraDistance = value; }

    // References
    public CharacterController CharacterController { get; private set; }
    public Animator Animator { get; private set; }
    public LivingEntity LivingEntity { get; private set; }
    public CinemachinePositionComposer CinemachinePositionComposer { get; private set; }
    
    // Animation IDs
    // private int _animationIdVelocitySide = Animator.StringToHash("velocitySide");
    // private int _animationIdVelocityFront = Animator.StringToHash("velocityFront");
    // private int _animationIdMoving = Animator.StringToHash("moving");
    // private int _animationIdSprinting = Animator.StringToHash("sprinting");
    // private int _animationIdJumping = Animator.StringToHash("jumping");

    private InputAction _interactAction;
    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        LivingEntity = GetComponent<LivingEntity>();
        CinemachinePositionComposer = CameraObject.GetComponent<CinemachinePositionComposer>();
        
        // _targetDirection = transform.forward;
        _cameraDistance = MinCameraDistance;

        _interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {


        recalculateStats();

        var movementVector = Quaternion.Euler(0, 45, 0) * new Vector3(_movementInputVector.x, 0, _movementInputVector.y);
        // CharacterController.SimpleMove(movementVector * LivingEntity.MovementSpeed); // nie dziala???
        CharacterController.Move(movementVector * Acceleration * Time.deltaTime);

        // temporary 
        if (movementVector != Vector3.zero) 
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movementVector), WalkingTurnSpeed * Time.deltaTime);

        if(_interactAction.WasPressedThisFrame())
        {
            InteractWith();
        }
    }

    private void recalculateStats() {
        VekhtarControl.Recalculate(LivingEntity.ModifierSystem);
    }

    // Input events

    void OnMove(InputValue value) {
        _movementInputVector = value.Get<Vector2>();
    }
    void OnLook(InputValue value) {
        Vector2 pointerVector = value.Get<Vector2>();
        pointerVector /= new Vector2(Screen.width, Screen.height);
        if (pointerVector.x < 0) {
            pointerVector.x = 0;
        }
        if (pointerVector.x > 1) {
            pointerVector.x = 1;
        }
        if (pointerVector.y < 0) {
            pointerVector.y = 0;
        }
        if (pointerVector.y > 1) {
            pointerVector.y = 1;
        }

        pointerVector *= 2;
        pointerVector -= Vector2.one;

        var pointerVectorPos = pointerVector * CameraTargetObjectBounds;

        CameraTargetObject.transform.localPosition = new Vector3(
            pointerVectorPos.x,
            CameraTargetObject.transform.localPosition.y,
            pointerVectorPos.y
        );
    }
    
    void OnScrollWheel(InputValue value) {
        var delta = value.Get<Vector2>();
        _cameraDistance -= delta.y * CameraDistanceSpeed;
        if (_cameraDistance < MinCameraDistance) {
            _cameraDistance = MinCameraDistance;
        }
        if (_cameraDistance > MaxCameraDistance) {
            _cameraDistance = MaxCameraDistance;
        }
    }

    void OnToggleInventory(InputValue value) {
        InventoryToggleEvent?.Invoke();
    }

    void OnCancel(InputValue value) {
        UICancelEvent?.Invoke();
    }

    void OnRotateItem(InputValue value) {
        ItemRotateEvent?.Invoke();
    }

    // Animation events

    void OnFootstep() {
        Debug.Log("Footstep");
    }

    void OnLand() {
        Debug.Log("Land");
    }

    void OnPlayerDeath() {
        Debug.Log("Player died");
    }

    private void InteractWith()
    {
        float interactRange = 2f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach(Collider c in colliderArray)
        {
            if(c.TryGetComponent(out IInteractable i))
            {
                i.Interact(this);
            }
        }
    }
}
