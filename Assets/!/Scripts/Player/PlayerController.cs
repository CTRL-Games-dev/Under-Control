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
[RequireComponent(typeof(HumanoidInventory))]
public class PlayerController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float _acceleration = 2f;
    [SerializeField] private float _deceleration = 2f;
    public float _currentSpeed = 0f;
    [SerializeField] private float _maxWalkingSpeed = 2f;
    [SerializeField] private float _maxSprintingSpeed = 4f;
    
    [SerializeField] private float _turnSpeed = 1f;
    
    public float MinCameraDistance = 10f;
    public float MaxCameraDistance = 30f;
    public float CameraDistanceSpeed = 1f;
    public float MaxInteractionRange = 10f;
    public Vector2 CameraTargetObjectBounds = Vector2.zero;
    public GameObject CameraObject;
    public GameObject CameraTargetObject;

    [Header("Weapon")]
    public WeaponHolder WeaponHolder;
    public WeaponItemData FistWeaponData;
    public WeaponItemData CurrentWeapon {
        get {
            HumanoidInventory inventory = LivingEntity.Inventory as HumanoidInventory;

            if (inventory.Weapon == null) {
                return FistWeaponData;
            }

            return inventory.Weapon;
        }
    }

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
    private bool _isAttacking = false;

    private readonly int _speedHash = Animator.StringToHash("speed");
    private readonly int _dodgeHash = Animator.StringToHash("dodge");
    private readonly int _lightAttackHash = Animator.StringToHash("light_attack");
    private readonly int _heavyAttackHash = Animator.StringToHash("heavy_attack");

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
        
        _cameraDistance = MinCameraDistance;

        WeaponHolder.UpdateWeapon(CurrentWeapon);
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

    void OnDodge() {
        Animator.SetTrigger(_dodgeHash);
    }

    void OnPrimaryInteraction() {
        interact(true);
    }

    void OnSecondaryInteraction() {
        interact(false);
    }

    private void interact(bool primary) {
        bool interacted = tryInteract();
        
        if(interacted) return;

        // Default to attacking if no interaction was commited
        if(primary) {
            performLightAttack();
        } else {
            performHeavyAttack();
        }
    }

    private bool tryInteract() {
        Ray ray = CameraObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) {
            return false;
        }

        Transform objectHit = hit.transform;

        // Check if object is to far
        if(Vector3.Distance(objectHit.position, transform.position) > MaxInteractionRange) {
            return false;
        }

        if(!objectHit.TryGetComponent(out IInteractable i)) {
            return false;
        }

        i.Interact(this);

        return true;
    }

    private void performLightAttack() {
        Animator.SetTrigger(_lightAttackHash);
    }

    private void performHeavyAttack() {
        Animator.SetTrigger(_heavyAttackHash);
    }

    public void OnWeaponHit(LivingEntity target) {
        if(!_isAttacking) return;
        if(target == LivingEntity) return;

        if(CurrentWeapon.DamageMax <= 0) {
            Debug.LogWarning($"DamageMax is zero or negative. Current weapon is {CurrentWeapon.DisplayName}");
            return;
        }

        if(CurrentWeapon.DamageMin < 0) {
            Debug.LogWarning($"DamageMin is negative. Current weapon is {CurrentWeapon.DisplayName}");
            return;
        }

        if(CurrentWeapon.DamageMax < CurrentWeapon.DamageMin) {
            Debug.LogWarning($"DamageMax ({CurrentWeapon.DamageMax}) is less than DamageMin ({CurrentWeapon.DamageMin}). Current weapon is {CurrentWeapon.DisplayName}");
            return;
        }

        float damageValue = UnityEngine.Random.Range(CurrentWeapon.DamageMin, CurrentWeapon.DamageMax);

        target.TakeDamage(new Damage{
            Type = CurrentWeapon.DamageType,
            Value = damageValue
        }, LivingEntity);
    }

    public void OnInventoryChanged() {
        WeaponHolder.UpdateWeapon(CurrentWeapon);
    }

    public void OnAttackAnimationStart() {
        _isAttacking = true;
    }

    public void OnAttackAnimationEnd() {
        _isAttacking = false;
    }
}
