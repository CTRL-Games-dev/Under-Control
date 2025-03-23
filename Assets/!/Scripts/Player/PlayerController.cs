using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
    public GameObject MainCameraObject;
    public GameObject CinemachineObject;
    public GameObject CameraTargetObject;
    public CinemachineCamera PlayerTopDownCamera;
    public bool InputDisabled = true;
    public bool DamageDisabled = false;
    public float LightAttackSpeed = 1f;
    public float HeavyAttackSpeed = 1f;
    public float DashDistance = 2f;
    public float DashCooldown = 1f;
    public float DashDuration = 0.2f;
    [SerializeField] private ParticleSystem[] _trailParticles;

    [Header("Weapon")]
    public WeaponHolder WeaponHolder;
    public WeaponItemData FistsWeaponData;
    public WeaponItemData CurrentWeapon {
        get {
            HumanoidInventory inventory = LivingEntity.Inventory as HumanoidInventory;

            if (inventory.Weapon == null) {
                return FistsWeaponData;
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
    private bool _isAttacking = false;
    private bool _lockRotation = false;
    private bool _canRotateOnClick = false;
    private bool _canDodge = true;


    [Header("Events")]
    public UnityEvent InventoryToggleEvent;
    public UnityEvent UICancelEvent;
    public UnityEvent ItemRotateEvent;
    public UnityEvent<int> CoinsChangeEvent;

    // State
    private Vector2 _movementInputVector = Vector2.zero;
    private float _cameraDistance { get => CinemachinePositionComposer.CameraDistance; set => CinemachinePositionComposer.CameraDistance = value; }
    private InteractionType? _queuedInteraction;

    private readonly int _speedHash = Animator.StringToHash("speed");
    private readonly int _lightAttackHash = Animator.StringToHash("attack_light");
    private readonly int _heavyAttackHash = Animator.StringToHash("attack_heavy");
    private readonly int _weaponTypeHash = Animator.StringToHash("weapon_type");
    private readonly int _lightAttackSpeedHash = Animator.StringToHash("attack_light_speed");
    private readonly int _heavyAttackSpeedHash = Animator.StringToHash("attack_heavy_speed");

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
        CinemachinePositionComposer = CinemachineObject.GetComponent<CinemachinePositionComposer>();
        LivingEntity.OnDeath.AddListener(onDeath);
        
        _cameraDistance = MinCameraDistance;

        WeaponHolder.UpdateWeapon(CurrentWeapon);
    }

    void Update()
    {
        recalculateStats();

        // Nie mozna playerinputa wylaczyc? - nie mozna :)
        if (InputDisabled) {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, _deceleration * Time.deltaTime);
            return;
        }

        float goalSpeed = Input.GetKey(KeyCode.LeftShift) ? _maxSprintingSpeed : _maxWalkingSpeed; // do zmiany

        var movementVector = Quaternion.Euler(0, 45, 0) * new Vector3(_movementInputVector.x, 0, _movementInputVector.y);
        if (movementVector.magnitude > 0.1) {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, goalSpeed, _acceleration * Time.deltaTime);
        } else {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, _deceleration * Time.deltaTime);
        }

        handleInteraction();
        if (!_lockRotation) handleRotation();
    }

    void FixedUpdate()
    {
        Animator.SetFloat(_speedHash, _currentSpeed / _maxSprintingSpeed);
        Animator.SetFloat(_lightAttackSpeedHash, LightAttackSpeed);
        Animator.SetFloat(_heavyAttackSpeedHash, HeavyAttackSpeed);
    }

    private void handleRotation() {
        if (_movementInputVector.magnitude > 0.1f) {
            var targetRotation = Quaternion.Euler(0, 45, 0) * Quaternion.LookRotation(new Vector3(_movementInputVector.x, 0, _movementInputVector.y));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
        }
    }

    private void recalculateStats() {
        VekhtarControl.Recalculate(LivingEntity.ModifierSystem);
    }

    private void onDeath() {
        UICanvas.Instance.OpenUIState(UIState.DeathScreen);
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

    void OnPrimaryInteraction(InputValue value) {
        _queuedInteraction = InteractionType.Primary;
    }

    void OnSecondaryInteraction(InputValue value) {
        _queuedInteraction = InteractionType.Secondary;
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
        if (_isAttacking || InputDisabled) return;
        
        if (!_canDodge) {
            Debug.Log("Can't dodge yet");
            return;
        }

        _canDodge = false;
        Invoke(nameof(enableDodging), DashCooldown);
        _lockRotation = true;
        DamageDisabled = true;

        if (_movementInputVector.magnitude > 0.1f)
            transform.rotation = Quaternion.Euler(0, 45, 0) * Quaternion.LookRotation(new Vector3(_movementInputVector.x, 0, _movementInputVector.y));
        
        Animator.speed = 0;
        Animator.applyRootMotion = false;
        Animator.SetBool(_lightAttackHash, false);
        Animator.SetBool(_heavyAttackHash, false);


        foreach (ParticleSystem trail in _trailParticles) { trail.Play(); }
        transform.DOMove(transform.position + transform.forward * DashDistance, DashDuration).SetEase(Ease.OutQuint).OnComplete(() => {
            Animator.applyRootMotion = true;
            Animator.speed = 1;
            DamageDisabled = false;
            _lockRotation = false;
            foreach (ParticleSystem trail in _trailParticles) {
                trail.Clear();
                trail.Stop();
            }
        });
    }

    private void enableDodging() {
        _canDodge = true;
    }

    private void handleInteraction() {
        if(_queuedInteraction == null) return;

        interact(_queuedInteraction.Value);

        _queuedInteraction = null;
    }

    private void interact(InteractionType interactionType) {
        if(EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if(UICanvas.Instance.CurrentUIState != UIState.NotVisible) return;

        bool interacted = tryInteract(interactionType);
        
        if(interacted) return;

        // Default to attacking if no interaction was commited
        if(_isAttacking) return;
            
        if (_canRotateOnClick) {
            Vector2 dir = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y)) * Quaternion.Euler(0, 45, 0);
            transform.DORotateQuaternion(targetRotation, 0.05f).SetEase(Ease.OutSine);
        } 

        _lockRotation = true;
        switch(interactionType) {
            case InteractionType.Primary:
                performLightAttack();
                break;
            case InteractionType.Secondary:
                performHeavyAttack();
                break;
        };
        

    }

    private bool tryInteract(InteractionType interactionType) {
        Ray ray = UICanvas.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
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
        Animator.SetBool(_heavyAttackHash, false);
        Animator.SetTrigger(_lightAttackHash);
    }

    private void performHeavyAttack() {
        Animator.SetBool(_lightAttackHash, false);
        Animator.SetTrigger(_heavyAttackHash);
    }

    public void OnInventoryChanged() {
        WeaponHolder.UpdateWeapon(CurrentWeapon);
        Animator.SetInteger(_weaponTypeHash, (int)CurrentWeapon.WeaponType);
    }


    // calluje sie z animacji jesli dodane jest AttackAnimationBehaviour w animatorze

    #region Animation Messages
    public void OnStartAttackAnimationsEnter() {
        _isAttacking = true;
        WeaponHolder.BeginAttack();
    }

    public void OnEndAttackAnimationsEnter() {
        WeaponHolder.EndAttack();
        _isAttacking = false;
    }

    public void OnDealDamageAnimationEnter() {
        WeaponHolder.EnableHitbox();
    }

    public void OnDealDamageAnimationExit() {
        WeaponHolder.DisableHitbox();
    }


    // public void OnDodgeAnimationStart() {
    //     _isDodging = true;
    //     Animator.SetBool(_lightAttackHash, false);
    //     Animator.SetBool(_heavyAttackHash, false);
    //     DamageDisabled = true;
    // }


    public void OnLockRotationAnimationEnter() {
        _lockRotation = true;
    }

    public void OnUnLockRotationAnimationEnter() {
        _lockRotation = false;
        _canRotateOnClick = true;
    }

    public void OnLockRotationOnClickAnimationEnter() {
        _canRotateOnClick = false;
    }

    public void OnUnLockRotationOnClickAnimationEnter() {
        _canRotateOnClick = true;
    }

    #endregion
}
