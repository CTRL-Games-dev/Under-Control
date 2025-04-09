using System.Collections.Generic;
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
public class Player : MonoBehaviour {
    public static Player Instance;

    [Header("Stats")]
    public Stat Health => LivingEntity.Health;
    public Stat MaxHealth => LivingEntity.MaxHealth;
    public Stat RegenRate => LivingEntity.RegenRate;
    public Stat VekhtarControl = new DynamicStat(StatType.VEKTHAR_CONTROL, 0);
    public Stat MaxMana = new Stat(StatType.MAX_MANA, 100f);
    public Stat Mana = new Stat(StatType.MANA, 100f);
    public Stat Armor = new Stat(StatType.ARMOR, 0f);

    // public Stat LightAttackDamage = new Stat(StatType.LIGHT_ATTACK_DAMAGE, 10f);
    public Stat LightAttackSpeed = new Stat(StatType.LIGHT_ATTACK_SPEED, 1f);
    // public Stat LightAttackRange = new Stat(StatType.LIGHT_ATTACK_RANGE, 1f);

    // public Stat HeavyAttackDamage = new Stat(StatType.HEAVY_ATTACK_DAMAGE, 20f);
    public Stat HeavyAttackSpeed = new Stat(StatType.HEAVY_ATTACK_SPEED, 1f);
    // public Stat HeavyAttackRange = new Stat(StatType.HEAVY_ATTACK_RANGE, 1f);

    public Stat MovementSpeed = new Stat(StatType.MOVEMENT_SPEED, 10f);

    public Stat DashSpeed = new Stat(StatType.DASH_SPEED, 10f);
    public Stat DashCooldown = new Stat(StatType.DASH_COOLDOWN, 5f);
    public Stat DashDistance = new Stat(StatType.DASH_DISTANCE, 5f);

    // Coins
    [SerializeField] private int _coins = 0;
    public int Coins { 
        get{ return _coins; } 
        set {   
            CoinsChangeEvent?.Invoke(value - _coins);
            _coins = value; 
        }
    }

    [Header("Properties")]
    [SerializeField] private float _acceleration = 8f;
    [SerializeField] private float _deceleration = 4f;
    [SerializeField] private float _currentSpeed = 0f;
    [SerializeField] private float _turnSpeed = 260f;

    public float MaxMovementSpeed = 10;
    
    public float MinCameraDistance = 10f;
    public float MaxCameraDistance = 30f;
    public float CameraDistanceSpeed = 1f;
    public float MaxInteractionRange = 10f;
    public Vector2 CameraTargetObjectBounds = Vector2.zero;
    public GameObject MainCameraObject;
    public GameObject CinemachineObject;
    public GameObject CameraTargetObject;
    public CinemachineCamera TopDownCamera;
    public Camera MainCamera;
    public bool InputDisabled = true;
    public bool DamageDisabled = false;

    private int _evolutionPoints = 4;
    public int EvolutionPoints {
        get{ return _evolutionPoints; }
        set {
            _evolutionPoints = value;
            UICanvas.InventoryCanvas.ChangeEvoPoints();
        }
    }
    public List<EvoUI> SelectedEvolutions;

    [SerializeField] private UICanvas _uiCanvas;
    [SerializeField] private ParticleSystem[] _trailParticles;

    [Header("Weapon")]
    public WeaponHolder WeaponHolder;
    public WeaponItemData CurrentWeapon { get => Inventory.Weapon; }

    private bool _isAttacking = false;
    private bool _lockRotation = false;

    [Header("Events")]
    public UnityEvent InventoryToggleEvent;
    public UnityEvent UICancelEvent;
    public UnityEvent ItemRotateEvent;
    public UnityEvent<EvoUI> OnEvolutionSelected = new();
    [HideInInspector] public UnityEvent<int> CoinsChangeEvent;

    // State
    private Vector2 _movementInputVector = Vector2.zero;
    private float _cameraDistance { get => CinemachinePositionComposer.CameraDistance; set => CinemachinePositionComposer.CameraDistance = value; }
    private InteractionType? _queuedInteraction;
    private Cooldown dashCooldown = new Cooldown(0);

    private List<Modifier> _currentRingModifiers;
    private List<Modifier> _currentAmuletModifiers;

    // Animator ids
    private readonly int _speedHash = Animator.StringToHash("speed");
    private readonly int _lightAttackHash = Animator.StringToHash("attack_light");
    private readonly int _heavyAttackHash = Animator.StringToHash("attack_heavy");
    private readonly int _weaponTypeHash = Animator.StringToHash("weapon_type");
    private readonly int _lightAttackSpeedHash = Animator.StringToHash("attack_light_speed");
    private readonly int _heavyAttackSpeedHash = Animator.StringToHash("attack_heavy_speed");
    private readonly int _movementSpeedHash = Animator.StringToHash("movement_speed");

    [Header("References")]
    public static LivingEntity LivingEntity { get; private set; }
    public static ModifierSystem ModifierSystem { get; private set; }
    public static UICanvas UICanvas { get => Instance._uiCanvas; }
    public static CharacterController CharacterController { get; private set; }
    public static Animator Animator { get; private set; }
    public static CinemachinePositionComposer CinemachinePositionComposer { get; private set; }
    public static HumanoidInventory Inventory => LivingEntity.Inventory as HumanoidInventory;

    [SerializeField] private LayerMask _groundLayerMask;
    public AnimationState CurrentAnimationState = AnimationState.Locomotion;

    

    #region Unity Methods
    void Awake() {
        LivingEntity = GetComponent<LivingEntity>();
        ModifierSystem = GetComponent<ModifierSystem>();
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        CinemachinePositionComposer = CinemachineObject.GetComponent<CinemachinePositionComposer>();

        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LivingEntity.OnDeath.AddListener(onDeath);
        MovementSpeed.OnValueChanged.AddListener(onMovementSpeedChanged);
        
        _cameraDistance = MinCameraDistance;

        registerStats();

        if (CurrentWeapon != null) {
            WeaponHolder.UpdateWeapon(CurrentWeapon);
        }

        OnEvolutionSelected.AddListener((evoUI) => {
            foreach (Modifier modifier in evoUI.GetModifiers()) {
                LivingEntity.ApplyIndefiniteModifier(modifier);
            }
        });

        // Amulet modifiers
        Inventory.OnInventoryChanged.AddListener(() => {
            foreach(var modifier in _currentAmuletModifiers) {
                LivingEntity.RemoveModifier(modifier);
            }

            _currentAmuletModifiers = Inventory.Amulet?.Modifiers;

            foreach(var modifier in _currentAmuletModifiers) {
                LivingEntity.ApplyIndefiniteModifier(modifier);
            }
        });

        // Ring modifiers
        Inventory.OnInventoryChanged.AddListener(() => {
            foreach(var modifier in _currentRingModifiers) {
                LivingEntity.RemoveModifier(modifier);
            }

            _currentRingModifiers = Inventory.Ring?.Modifiers;

            foreach(var modifier in _currentRingModifiers) {
                LivingEntity.ApplyIndefiniteModifier(modifier);
            }
        });

        // ResetRun();
    }

    void Update() {
        // Nie mozna playerinputa wylaczyc? - nie mozna :)
        if (InputDisabled) {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, _deceleration * Time.deltaTime);
            return;
        }

        // transform.rotation = Quaternion.LookRotation(new Vector3(Input.mousePosition.x, 1, Input.mousePosition.y));

        float goalSpeed = Input.GetKey(KeyCode.LeftShift) ? MaxMovementSpeed : MaxMovementSpeed / 2; // do zmiany

        var movementVector = Quaternion.Euler(0, 45, 0) * new Vector3(_movementInputVector.x, 0, _movementInputVector.y);
        if (movementVector.magnitude > 0.1 || _isAttacking) {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, goalSpeed, _acceleration * Time.deltaTime);
        } else {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, _deceleration * Time.deltaTime);
        }

        handleInteraction();

        if (!_lockRotation) handleRotation();

        CharacterController.SimpleMove(movementVector * _currentSpeed);
    }

    void FixedUpdate() {
        Animator.SetFloat(_speedHash, _currentSpeed / MaxMovementSpeed);
        Animator.SetFloat(_lightAttackSpeedHash, Instance.LightAttackSpeed);
        Animator.SetFloat(_heavyAttackSpeedHash, Instance.HeavyAttackSpeed);
    }

    #endregion

    private void handleRotation() {
        if (_movementInputVector.magnitude > 0.1f) {
            var targetRotation = Quaternion.Euler(0, 45, 0) * Quaternion.LookRotation(new Vector3(_movementInputVector.x, 0, _movementInputVector.y));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
        }
    }

    private void onDeath() {
        UICanvas.ChangeUITopState(UITopState.Death);
    }

    // Input events
    #region Input Events
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

        dashCooldown.CooldownTime = DashCooldown;
        
        if (!dashCooldown.Execute()) {
            return;
        }

        _lockRotation = true;
        DamageDisabled = true;

        if (_movementInputVector.magnitude > 0.1f) {
            transform.rotation = Quaternion.Euler(0, 45, 0) * Quaternion.LookRotation(new Vector3(_movementInputVector.x, 0, _movementInputVector.y));
        }
        
        Animator.speed = 0;
        // Animator.applyRootMotion = false;
        Animator.SetBool(_lightAttackHash, false);
        Animator.SetBool(_heavyAttackHash, false);

        foreach (ParticleSystem trail in _trailParticles) { trail.Play(); }

        transform.DOMove(transform.position + transform.forward * Instance.DashDistance, Instance.DashSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            // Animator.applyRootMotion = true;
            Animator.speed = 1;
            DamageDisabled = false;
            _lockRotation = false;
            foreach (ParticleSystem trail in _trailParticles) {
                trail.Clear();
                trail.Stop();
            }
        });
    }
    #endregion

    #region Interaction Methods

    private void handleInteraction() {
        if(_queuedInteraction == null) return;

        interact(_queuedInteraction.Value);

        _queuedInteraction = null;
    }

    private void interact(InteractionType interactionType) {
        if(EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if(UICanvas.CurrentUIMiddleState != UIMiddleState.NotVisible || UICanvas.CurrentUITopState != UITopState.NotVisible) return;

        bool interacted = tryInteract(interactionType);
        
        if(interacted) return;
        if(CurrentWeapon == null) return;

        // Default to attacking if no interaction was commited
        if(_isAttacking) return;
            
        if (!_lockRotation) {
            transform.LookAt(getMousePosition().position);
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
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
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

        i.Interact();

        return true;
    }

    private void performLightAttack() {
        WeaponHolder.InitializeAttack(AttackType.LIGHT);
        Animator.SetBool(_heavyAttackHash, false);
        Animator.SetTrigger(_lightAttackHash);
    }

    private void performHeavyAttack() {
        WeaponHolder.InitializeAttack(AttackType.HEAVY);
        Animator.SetBool(_lightAttackHash, false);
        Animator.SetTrigger(_heavyAttackHash);
    }

    public void OnInventoryChanged() {
        WeaponHolder.UpdateWeapon(CurrentWeapon);
        Animator.SetInteger(_weaponTypeHash, (int) (CurrentWeapon?.WeaponType ?? WeaponType.None));
    }


    private (bool success, Vector3 position) getMousePosition() {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayerMask)) {
            return (true, hit.point);
        } else {
            return (false, Vector3.zero);
        }

    }

    // calluje sie z animacji jesli dodane jest AttackAnimationBehaviour w animatorze

    #endregion

    #region Animation Messages
    // public void OnStartAttackAnimationsEnter() {
    //     _isAttacking = true;
    //     WeaponHolder.BeginAttack();
    // }

    // public void OnEndAttackAnimationsEnter() {
    //     WeaponHolder.EndAttack();
    //     _isAttacking = false;
    // }

    // public void OnDealDamageAnimationEnter() {
    //     WeaponHolder.EnableHitbox();
    // }

    // public void OnDealDamageAnimationExit() {
    //     WeaponHolder.DisableHitbox();
    // }


    // // public void OnDodgeAnimationStart() {
    // //     _isDodging = true;
    // //     Animator.SetBool(_lightAttackHash, false);
    // //     Animator.SetBool(_heavyAttackHash, false);
    // //     DamageDisabled = true;
    // // }


    // public void OnLockRotationAnimationEnter() {
    //     _lockRotation = true;
    // }

    // public void OnUnLockRotationAnimationEnter() {
    //     _lockRotation = false;
    //     _canRotateOnClick = true;
    // }

    // public void OnLockRotationOnClickAnimationEnter() {
    //     _canRotateOnClick = false;
    // }

    // public void OnUnLockRotationOnClickAnimationEnter() {
    //     _canRotateOnClick = true;
    // }

    #endregion

    #region Combat Methods

    public enum AnimationState {
        Locomotion,  
        Attack_Windup,
        Attack_Contact,
        Attack_ComboWindow,
        Attack_Recovery
    }

    public void SetAnimationState(AnimationState state) {
        exitAnimationState(CurrentAnimationState);
        enterAnimationState(state);
    }

    private void exitAnimationState(AnimationState state) {
        switch (state) {
            case AnimationState.Locomotion:
                break;

            case AnimationState.Attack_Windup:
                break;

            case AnimationState.Attack_Contact:
                _isAttacking = false;
                _lockRotation = false;
                Animator.applyRootMotion = false;
                WeaponHolder.EndAttack();
                WeaponHolder.DisableHitbox();
                break;

            case AnimationState.Attack_ComboWindow:
                break;

            case AnimationState.Attack_Recovery:
                break;
        }
    }

    private void enterAnimationState(AnimationState state) {
        CurrentAnimationState = state;
        switch (state) {
            case AnimationState.Locomotion:
                _lockRotation = false;
                break;

            case AnimationState.Attack_Windup:
                _lockRotation = true;
                _isAttacking = true;
                Animator.applyRootMotion = true;
                WeaponHolder.BeginAttack();
                break;

            case AnimationState.Attack_Contact:
                WeaponHolder.EnableHitbox();
                break;

            case AnimationState.Attack_ComboWindow:
                break;

            case AnimationState.Attack_Recovery:
                break;
        }
    }


    #endregion

    #region Misc Methods

    public void ResetRun() {
        ModifierSystem.Reset();

        // Evolution modifiers
        foreach (EvoUI evoUI in SelectedEvolutions) {
            foreach (Modifier modifier in evoUI.GetModifiers()) {
                LivingEntity.ApplyIndefiniteModifier(modifier);
            }
        }

        // Amulet modifiers
        foreach(var modifier in _currentAmuletModifiers) {
            LivingEntity.ApplyIndefiniteModifier(modifier);
        }

        // Ring modifiers
        foreach(var modifier in _currentRingModifiers) {
            LivingEntity.ApplyIndefiniteModifier(modifier);
        }
    }

    private void registerStats() {
        ModifierSystem.RegisterStat(ref VekhtarControl);
        ModifierSystem.RegisterStat(ref MaxMana);
        ModifierSystem.RegisterStat(ref Mana);
        ModifierSystem.RegisterStat(ref Armor);
        // ModifierSystem.RegisterStat(ref LightAttackDamage);
        ModifierSystem.RegisterStat(ref LightAttackSpeed);
        // ModifierSystem.RegisterStat(ref LightAttackRange);
        // ModifierSystem.RegisterStat(ref HeavyAttackDamage);
        ModifierSystem.RegisterStat(ref HeavyAttackSpeed);
        // ModifierSystem.RegisterStat(ref HeavyAttackRange);
        ModifierSystem.RegisterStat(ref MovementSpeed);
        ModifierSystem.RegisterStat(ref DashSpeed);
        ModifierSystem.RegisterStat(ref DashCooldown);
        ModifierSystem.RegisterStat(ref DashDistance);
    }

    private void onMovementSpeedChanged() {
        // Calculates movement speed multiplier
        Animator.SetFloat(_movementSpeedHash, MovementSpeed / MaxMovementSpeed);
    }

    public void SetPlayerPosition(Vector3 position, float time = 0, float yRotation = 45) {
        // Animator.applyRootMotion = false;
        Animator.speed = 0;
        gameObject.transform.position = position;
        gameObject.transform.DORotate(new Vector3(0, yRotation, 0), time).SetEase(Ease.OutSine).OnComplete(() => {
            applyRootMotion();
        });
    }

    private void applyRootMotion() {
        // Animator.applyRootMotion = true;
        Animator.speed = 1;
    }

    #endregion
}
