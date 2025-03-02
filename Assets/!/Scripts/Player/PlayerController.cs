using System;
using System.Collections;
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
    [SerializeField] private float _acceleration = 2f;
    [SerializeField] private float _deceleration = 2f;
    public float _currentSpeed = 0f;
    [SerializeField] private float _maxWalkingSpeed = 2f;
    [SerializeField] private float _maxSprintingSpeed = 4f;
    
    [SerializeField] private float _turnSpeed = 1f;
    
    public float MouseSensitivity = 0.1f; // po co 
    
    public float MinCameraDistance = 10f;
    public float MaxCameraDistance = 30f;
    public float CameraDistanceSpeed = 1f;
    public Vector2 CameraTargetObjectBounds = Vector2.zero;
    public GameObject CameraObject;
    public GameObject CameraTargetObject;

    // Attack 
    public float LightAttackDamage = 10f;
    public float ChargeAttackDamageMultiplier = 3f;
    public Cooldown ComboCooldown;

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

    // State
    private Vector2 _movementInputVector = Vector2.zero;
    private float _cameraDistance { get => CinemachinePositionComposer.CameraDistance; set => CinemachinePositionComposer.CameraDistance = value; }
    private bool _lightAttack;
    private bool _chargeAttack;
    private int _comboCounter;

    // References
    public CharacterController CharacterController { get; private set; }
    public Animator Animator { get; private set; }
    public LivingEntity LivingEntity { get; private set; }
    public CinemachinePositionComposer CinemachinePositionComposer { get; private set; }
    
    readonly private int _speedHash = Animator.StringToHash("speed");
    readonly private int _dodgeHash = Animator.StringToHash("dodge");
    readonly private int _lightAttackHash = Animator.StringToHash("light_attack");
    readonly private int _heavyAttackHash = Animator.StringToHash("heavy_attack");


    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        LivingEntity = GetComponent<LivingEntity>();
        CinemachinePositionComposer = CameraObject.GetComponent<CinemachinePositionComposer>();
        
        _cameraDistance = MinCameraDistance;
    }

    void Update()
    {
        recalculateStats();

        float goalSpeed = Input.GetKey(KeyCode.LeftShift) ? _maxSprintingSpeed : _maxWalkingSpeed; // do zmiany

        var movementVector = Quaternion.Euler(0, 45, 0) * new Vector3(_movementInputVector.x, 0, _movementInputVector.y);
        if (movementVector.magnitude > 0.1) {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, goalSpeed, _acceleration * Time.deltaTime);
        } else {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, _deceleration * Time.deltaTime);
        }

        handleRotation();
        // CharacterController.Move(transform.forward * _currentSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        Animator.SetFloat(_speedHash, _currentSpeed / _maxSprintingSpeed);
    }

    private void handleRotation() {
        if (_movementInputVector.magnitude > 0.1f) {
            var targetRotation = Quaternion.Euler(0, 45, 0) * Quaternion.LookRotation(new Vector3(_movementInputVector.x, 0, _movementInputVector.y));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
        }
    }

    private void recalculateStats() {
        VekhtarControl.Recalculate(LivingEntity.ModifierSystem);
    }

    // Handle attack logic
    // TODO: Finish attack login soon
    private void handleAttack() {
        if (_lightAttack)
        {
            if (_comboCounter < 3) {
                print("Light attack" + _comboCounter);
            }

            _comboCounter++;    
            _lightAttack = false;                    
        }

        if (_comboCounter == 4)
        {
            print("Knockback attack");
            _comboCounter = 0;
        }

        if (_chargeAttack && _comboCounter == 0) 
        {
            print("Charge attack");
            _chargeAttack = false;
        }
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

    // void OnLightAttack(InputValue value) {
    //     //_animator.SetTrigger("lightAttack");
    //     _lightAttack = value.isPressed;
    // }

    // void OnChargeAttack(InputValue value) {
    //     //_animator.SetTrigger("chargeAttack");
    //     _chargeAttack = value.isPressed;        
    // }


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

    // Totalnie do zmiany, potrzebujemy interakcji myszka
    // Kyśnij się London
    // void OnInteract(InputValue value)
    // {
    //     float interactRange = 2f;
    //     Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
    //     foreach(Collider c in colliderArray)
    //     {
    //         if(c.TryGetComponent(out IInteractable i))
    //         {
    //             i.Interact(this);
    //         }
    //     }
    // }

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


    void OnDodge() {
        Animator.SetTrigger(_dodgeHash);
    }

    void OnLightAttack() {
        Animator.SetTrigger(_lightAttackHash);
    }

    void OnHeavyAttack() {
        Animator.SetTrigger(_heavyAttackHash);
    }
}
