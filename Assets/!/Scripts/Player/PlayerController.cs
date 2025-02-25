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
    public float Acceleration = 2f;
    public float Deceleration = 2f;
    public float MaxWalkingSpeed = 1f;
    public float MaxSprintingSpeed = 2f;
    public float MouseSensitivity = 0.1f;
    public float WalkingTurnSpeed = 1f;
    public float MinCameraDistance = 10f;
    public float MaxCameraDistance = 30f;
    public float CameraDistanceSpeed = 1f;
    public Vector2 CameraTargetObjectBounds = Vector2.zero;
    public GameObject CameraObject;
    public GameObject CameraTargetObject;

    // Attack 
    public static float LightAttackDamage = 10f;
    public float ChargeAttackDamage = LightAttackDamage * 3;
    public float ComboGapTime = 1.0f;

    // State
    private Vector2 _movementInputVector = Vector2.zero;
    private Vector3 _targetDirection;
    private bool _sprinting = false;
    private float _velocitySide = 0;
    private float _velocityFront = 0;
    private bool _isTurning;
    private float _cameraDistance { get => _cinemachinePositionComposer.CameraDistance; set => _cinemachinePositionComposer.CameraDistance = value; }
    private bool _lightAttack;
    private bool _chargeAttack;
    private int _comboCounter;
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

    // References
    public CharacterController CharacterController { get; private set; }
    public Animator Animator { get; private set; }
    public LivingEntity LivingEntity { get; private set; }
    public CinemachinePositionComposer CinemachinePositionComposer { get; private set; }
    
    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        LivingEntity = GetComponent<LivingEntity>();
        CinemachinePositionComposer = CameraObject.GetComponent<CinemachinePositionComposer>();
        
        // _targetDirection = transform.forward;
        _cameraDistance = MinCameraDistance;
    }

    void Update()
    {
        recalculateStats();

        var movementVector = Quaternion.Euler(0, 45, 0) * new Vector3(_movementInputVector.x, 0, _movementInputVector.y);
        CharacterController.SimpleMove(movementVector * LivingEntity.MovementSpeed);
    }

    private void recalculateStats() {
        VekhtarControl.Recalculate(LivingEntity.ModifierSystem);
    }

    // Handle attack logic
    private void handleAttack() {
        if (_lightAttack)
        {
            StartCoroutine(ResetCombo());
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

    public IEnumerator ResetCombo() {
        float elapsedTime = 0f;

        while (elapsedTime < ComboGapTime) {
            if (_lightAttack) {
                elapsedTime = 0f;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _comboCounter = 0;
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

    void OnSprint(InputValue value) {
        _sprinting = value.isPressed;
        _animator.SetBool("sprinting", _sprinting);
    }

    /*void OnAttack(InputValue value) {
        _animator.SetTrigger("punch");
    }*/

    void OnLightAttack(InputValue value) {
        //_animator.SetTrigger("lightAttack");
        _lightAttack = value.isPressed;

    }

    void OnChargeAttack(InputValue value) {
        //_animator.SetTrigger("chargeAttack");
        _chargeAttack = value.isPressed;        
    }

    void OnDodge(InputValue value) {
        //_animator.SetTrigger("dodge");
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

    // Totalnie do zmiany, potrzebujemy interakcji myszka
    void OnInteract(InputValue value)
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
}
