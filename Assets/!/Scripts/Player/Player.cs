using System;
using System.Collections.Generic;
using System.Linq;
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
    [Serializable]
    private struct SpellData {
        public Spell Spell;
        public Cooldown Cooldown;

        public bool TryCast(LivingEntity caster) {
            if(Spell == null) return false;
            if(caster.Mana < Spell.Mana) return false;
            if(!Cooldown.Execute()) return false;

            Instance.CastSpell(Spell);

            caster.Mana -= Spell.Mana; 

            return true;
        }
    }

    public static Player Instance;

    [Header("Stats")]
    public float Health => LivingEntity.Health;
    public Stat MaxHealth => LivingEntity.MaxHealth;
    public Stat MaxMana => LivingEntity.MaxMana;
    public float Mana => LivingEntity.Mana;
    public Stat VekhtarControl = new DynamicStat(StatType.VEKTHAR_CONTROL, 0);
    public Stat Armor = new Stat(StatType.ARMOR, 0f);

    // public Stat LightAttackDamage = new Stat(StatType.LIGHT_ATTACK_DAMAGE, 10f);
    public Stat LightAttackSpeed = new Stat(StatType.LIGHT_ATTACK_SPEED, 1f);
    // public Stat LightAttackRange = new Stat(StatType.LIGHT_ATTACK_RANGE, 1f);

    // public Stat HeavyAttackDamage = new Stat(StatType.HEAVY_ATTACK_DAMAGE, 20f);
    public Stat HeavyAttackSpeed = new Stat(StatType.HEAVY_ATTACK_SPEED, 1f);
    // public Stat HeavyAttackRange = new Stat(StatType.HEAVY_ATTACK_RANGE, 1f);

    public Stat MovementSpeed = new Stat(StatType.MOVEMENT_SPEED, 10f);

    public Stat DashSpeedMultiplier = new Stat(StatType.DASH_SPEED_MULTIPLIER, 2f);
    public Stat DashCooldown = new Stat(StatType.DASH_COOLDOWN, 2f);
    public Stat DashDuration = new Stat(StatType.DASH_COOLDOWN, 0.3f);

    // Coins
    [SerializeField] private int _coins = 100;
    public int Coins { 
        get{ return _coins; } 
        set {   
            CoinsChangeEvent?.Invoke(value - _coins);
            _coins = value; 
        }
    }
    [Header("Sound effects")]
    AudioClip OnDashSound;
    [Header("Properties")]
    [SerializeField] private float _acceleration = 8f;
    [SerializeField] private float _deceleration = 4f;
    [SerializeField] private float _attackDeceleration = 12f;
    [SerializeField] private float _currentSpeed = 0f;
    [SerializeField] private float _turnSpeed = 260f;

    public float MinCameraDistance = 10f;
    public float MaxCameraDistance = 30f;
    public float CameraDistanceSpeed = 1f;
    public float MaxInteractionRange = 10f;
    public Vector2 CameraTargetObjectBounds = Vector2.zero;
    public GameObject MainCameraObject;
    public GameObject CinemachineObject;
    public GameObject CameraTargetObject;
    public CinemachineCamera TopDownCamera;
    public CinemachineBasicMultiChannelPerlin CameraNoise;
    public Camera MainCamera;
    public bool InputDisabled = true;
    public bool DamageDisabled = false;
    [SerializeField] private Material _dissolveMaterial;
    public Vector3 StartPosition;

    public int _evolutionPoints = 0;
    public int EvolutionPoints {
        get{ return _evolutionPoints; }
        set {
            _evolutionPoints = value;
            UICanvas.InventoryCanvas.ChangeEvoPoints();
        }
    }

    public List<EvoUI> SelectedEvolutions;

    [Header("Spells")]
    [SerializeField]
    private SpellData _spellDataOne; 
    public Spell SpellSlotOne {
        get => _spellDataOne.Spell;
        set {
            _spellDataOne.Spell = value;
            // _spellDataOne.Cooldown = new Cooldown(value.CooldownTime); 
            UICanvas.HUDCanvas.UpdateSpellSlots();
        }
    }

    [SerializeField]
    private SpellData _spellDataTwo;
    public Spell SpellSlotTwo {
        get => _spellDataTwo.Spell;
        set {
            _spellDataTwo.Spell = value;
            // _spellDataTwo.Cooldown = new Cooldown(value.CooldownTime);
            UICanvas.HUDCanvas.UpdateSpellSlots();
        }
    }

    [SerializeField]
    private SpellData _spellDataThree;
    public Spell SpellSlotThree {
        get => _spellDataThree.Spell;
        set {
            _spellDataThree.Spell = value;
            // _spellDataThree.Cooldown = new Cooldown(value.CooldownTime);
            UICanvas.HUDCanvas.UpdateSpellSlots();
        }
    }

    private Spell _queuedSpell = null;

    [Header("Consumable")]
    public InventoryItem<ConsumableItemData> ConsumableItemOne = null;
    public InventoryItem<ConsumableItemData> ConsumableItemTwo = null;
    public Cooldown ConsumableCooldown = new Cooldown(0.5f);

    [Header("Weapon")]
    public WeaponHolder WeaponHolder;
    public InventoryItem<WeaponItemData> CurrentWeapon { get => Inventory.Weapon; }

    private bool _isAttacking = false;
    public bool LockRotation = false;
    public bool UpdateDisabled = false;
    public bool HasPlayerDied = false;
    public SlashManager SlashManager;

    [Header("Events")]
    public UnityEvent InventoryToggleEvent;
    public UnityEvent UICancelEvent;
    public UnityEvent ItemRotateEvent;
    public UnityEvent<EvoUI> OnEvolutionSelected = new();
    public UnityEvent<int> CoinsChangeEvent;
    public UnityEvent UpdateConsumablesEvent;
    public float CameraDistance { get => CinemachinePositionComposer.CameraDistance; set => CinemachinePositionComposer.CameraDistance = value; }

    // State
    private Vector2 _movementInputVector = Vector2.zero;
    private InteractionType? _queuedInteraction;
    private Cooldown _dashCooldown = new Cooldown(0);

    private List<Modifier> _currentRingModifiers;
    private List<Modifier> _currentAmuletModifiers;

    // Animator ids
    private readonly int _speedHash = Animator.StringToHash("speed");
    private readonly int _lightAttackHash = Animator.StringToHash("attack_light");
    private readonly int _heavyAttackHash = Animator.StringToHash("attack_heavy");
    private readonly int _weaponTypeHash = Animator.StringToHash("weapon_type");
    private readonly int _lightAttackSpeedHash = Animator.StringToHash("attack_light_speed");
    private readonly int _heavyAttackSpeedHash = Animator.StringToHash("attack_heavy_speed");
    private readonly int _spellHash = Animator.StringToHash("spell");

    [Header("References")]
    [SerializeField] private UICanvas _uiCanvas;
    [SerializeField] private ParticleSystem[] _trailParticles;
    public GameObject FBXModel;

    // Static reference getters
    public static LivingEntity LivingEntity { get; private set; }
    public static ModifierSystem ModifierSystem { get; private set; }
    public static UICanvas UICanvas { get => Instance._uiCanvas; }
    public static CharacterController CharacterController { get; private set; }
    public static Animator Animator { get; private set; }
    public static CinemachinePositionComposer CinemachinePositionComposer { get; private set; }
    public static SpellSpawner SpellSpawner { get; private set; }
    public static HumanoidInventory Inventory => LivingEntity.Inventory as HumanoidInventory;

    [SerializeField] private LayerMask _groundLayerMask;
    private LayerMask _interactionMask;
    public FaceAnimator FaceAnimator;
    public AnimationState CurrentAnimationState = AnimationState.Locomotion;
    public InputActionAsset actions;
    private Vector3 _queuedRotation = Vector3.zero;
    public List<WeaponItemData> StarterWeapons = new List<WeaponItemData>();

    #region Unity Methods
    void Awake() {
        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        
        LivingEntity = GetComponent<LivingEntity>();
        ModifierSystem = GetComponent<ModifierSystem>();
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        CinemachinePositionComposer = CinemachineObject.GetComponent<CinemachinePositionComposer>();
        SpellSpawner = GetComponentInChildren<SpellSpawner>();

        Instance = this;

        LivingEntity.OnDeath.AddListener(onDeath);
        LivingEntity.OnStunned.AddListener(onStunned);
        EventBus.ItemPlacedEvent.AddListener(UpdateEquipment);
        
        CameraDistance = MinCameraDistance;

        registerStats();

        StartPosition = transform.position;

        OnEvolutionSelected.AddListener((evoUI) => {
            foreach (Modifier modifier in evoUI.GetModifiers()) {
                LivingEntity.ApplyIndefiniteModifier(modifier);
            }
        });

        LivingEntity.OnDamageTaken.AddListener((data) => {
            CameraManager.ShakeCamera(2, 0.1f);
        });
 
        _interactionMask |= 1 << LayerMask.NameToLayer("Interactable");

        // LoadKeybinds();
    }

    void Start() {
        if (CurrentWeapon.ItemData != null && CurrentWeapon != null) {
            WeaponHolder.UpdateWeapon(CurrentWeapon);
        }

        // Amulet modifiers
        Inventory.OnInventoryChanged.AddListener(() => {
            if (_currentAmuletModifiers != null) {
                foreach(var modifier in _currentAmuletModifiers) {
                    LivingEntity.RemoveModifier(modifier);
                }
            }

            _currentAmuletModifiers = Inventory.Amulet?.ItemData?.Modifiers;

            if (_currentAmuletModifiers != null) {
                foreach(var modifier in _currentAmuletModifiers) {
                    LivingEntity.ApplyIndefiniteModifier(modifier);
                }
            }
        });
        
        OnDashSound =  Resources.Load("SFX/bohater/dash") as AudioClip;

        ResetRun();
    }

    void Update() {
        if (UpdateDisabled) return;

        handleInteraction();
        handleRotation();

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, getGoalSpeed(), getSpeedChange() * Time.deltaTime);
        CharacterController.SimpleMove(getGoalDirection() * _currentSpeed);
    }

    void FixedUpdate() {
        // Magiczna liczba to predkosc animacji biegu
        Animator.SetFloat(_speedHash, _currentSpeed / Instance.MovementSpeed);
        Animator.SetFloat(_lightAttackSpeedHash, Instance.LightAttackSpeed);
        Animator.SetFloat(_heavyAttackSpeedHash, Instance.HeavyAttackSpeed);

        ModifierSystem.GetActiveModifiers();
    }

    #endregion

    private float getSpeedChange() {
        if (InputDisabled) return _deceleration;

        switch (CurrentAnimationState) {
            case AnimationState.Dash:
                return _acceleration * 100;
            case AnimationState.Locomotion:
                return _movementInputVector.magnitude > 0.1f ? _acceleration : _deceleration;
                
            case AnimationState.Attack_Windup:
            case AnimationState.Attack_Contact:
            case AnimationState.Attack_ComboWindow:
            case AnimationState.Attack_Recovery:
                return _attackDeceleration;

            default:
                return 0;
        }
    }

    private Vector3 getGoalDirection() {
        switch (CurrentAnimationState) {
            case AnimationState.Dash:
            case AnimationState.Attack_Windup:
                if (_queuedRotation == Vector3.zero) return transform.forward;
                Vector3 dir = _queuedRotation;
                _queuedRotation = Vector3.zero;
                return dir.normalized;
            case AnimationState.Attack_Contact:
            case AnimationState.Attack_ComboWindow:
                return transform.forward;

            case AnimationState.Locomotion:
            case AnimationState.Attack_Recovery:
            default:
                return Quaternion.Euler(0, 45, 0) * new Vector3(_movementInputVector.x, 0, _movementInputVector.y).normalized;
        }
    }

    private float getGoalSpeed() {
        if (InputDisabled) return 0;

        switch (CurrentAnimationState) {
            case AnimationState.Dash:
                return MovementSpeed * DashSpeedMultiplier;
            case AnimationState.Locomotion:
                return _movementInputVector.magnitude > 0.1f ? MovementSpeed : 0;

            case AnimationState.Attack_Windup:
            case AnimationState.Attack_Contact:
            case AnimationState.Attack_ComboWindow:
            case AnimationState.Attack_Recovery:
                return _movementInputVector.magnitude > 0.1f ? MovementSpeed * 0.3f : 0;

            case AnimationState.Spell_Cast:
                return 0;
        }

        return 0;
    }

    private void handleRotation() {
        if (LockRotation) return;
        if (_movementInputVector.magnitude > 0.1f) {
            var targetRotation = Quaternion.Euler(0, 45, 0) * Quaternion.LookRotation(new Vector3(_movementInputVector.x, 0, _movementInputVector.y));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
        }
    }

    private void onDeath() {
        if (HasPlayerDied) return;
        Debug.Log("Player died");
        GameManager.Instance.ShowMainMenu = false;
        HasPlayerDied = true;
        SlashManager.DisableSlash();
        _isAttacking = false;
        LockRotation = false;
        UICanvas.ChangeUITopState(UITopState.Death);
        Animator.SetTrigger("die");
        UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
    }

    private void onStunned(float duration) {
        CameraManager.ShakeCamera(2, duration);
    }

    
    #region Input Events

    void OnCastSpellOne(InputValue value) {
        if (SpellSlotOne == null) return;
        if(!_spellDataOne.TryCast(LivingEntity)) return;

         UICanvas.HUDCanvas.UseSpell1();
    }

    void OnCastSpellTwo(InputValue value) {
        if (SpellSlotTwo == null) return;
        if(!_spellDataTwo.TryCast(LivingEntity)) return;

        UICanvas.HUDCanvas.UseSpell2();
    }

    void OnCastSpellThree(InputValue value) {
        if (SpellSlotThree == null) return;
        if(!_spellDataThree.TryCast(LivingEntity)) return;

        UICanvas.HUDCanvas.UseSpell3();
    }

    void OnUseConsumableOne(InputValue value) {
        if(ConsumableItemOne == null) return;
        if(!ConsumableCooldown.Execute()) return;
        if(ConsumableItemOne.Amount <= 0) return;

        ConsumableItemData c =  ConsumableItemOne.ItemData;

        if(c == null) return;
        c.Consume(LivingEntity);
        ConsumableItemOne.Amount--;

        if (ConsumableItemOne.Amount <= 0) {
            ConsumableItemOne = null;
        }

        UICanvas.HUDCanvas.UseConsumable1();
        UpdateConsumablesEvent?.Invoke();
    }

    void OnUseConsumableTwo(InputValue value) {
        if(ConsumableItemTwo == null) return;
        if(!ConsumableCooldown.Execute()) return;
        if(ConsumableItemTwo.Amount <= 0) return;

        ConsumableItemData c =  ConsumableItemTwo.ItemData as ConsumableItemData;
        
        if(c == null) return;
        c.Consume(LivingEntity);
        ConsumableItemTwo.Amount--;
        if (ConsumableItemTwo.Amount <= 0) {
            ConsumableItemTwo = null;
        }

        UICanvas.HUDCanvas.UseConsumable2();
        UpdateConsumablesEvent?.Invoke();
    }

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
    void OnKeyboardInteraction(InputValue value){
        if(UICanvas.CurrentUIMiddleState != UIMiddleState.NotVisible || UICanvas.CurrentUITopState != UITopState.NotVisible || UICanvas.CurrentUIBottomState != UIBottomState.HUD) return;
        if(InputDisabled) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position,MaxInteractionRange, _interactionMask);
        if (colliders.Length == 0) return;
        colliders = colliders.OrderBy(x => Vector3.Distance(transform.position, x.gameObject.transform.position)).ToArray();
        IInteractable closestInteractable = colliders[0].GetComponent<IInteractable>();
        closestInteractable?.Interact();
    }

    void OnScrollWheel(InputValue value) {
        var delta = value.Get<Vector2>();
        CameraDistance -= delta.y * CameraDistanceSpeed;
        if (CameraDistance < MinCameraDistance) {
            CameraDistance = MinCameraDistance;
        }
        if (CameraDistance > MaxCameraDistance) {
            CameraDistance = MaxCameraDistance;
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

        _dashCooldown.CooldownTime = DashCooldown;
        
        if (!_dashCooldown.Execute()) {
            return;
        }

        UICanvas.HUDCanvas.ShowDashCooldown();

        LockRotation = true;
        DamageDisabled = true;

        if (_movementInputVector.magnitude > 0.1f) {
            transform.rotation = Quaternion.Euler(0, 45, 0) * Quaternion.LookRotation(new Vector3(_movementInputVector.x, 0, _movementInputVector.y));
        }
        
        Animator.speed = 0;
        // Animator.applyRootMotion = false;
        Animator.SetBool(_lightAttackHash, false);
        Animator.SetBool(_heavyAttackHash, false);

        foreach (ParticleSystem trail in _trailParticles) { trail.Play(); }

        CurrentAnimationState = AnimationState.Dash;
        SoundFXManager.Instance.PlaySoundFXClip(OnDashSound, transform,1.2f);
        

        // UpdateDisabled = true;
        // Animator.animatePhysics = false;

        // Instance.transform.DOMove(transform.position + transform.forward * Instance.DashDistance, Instance.DashSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
        //     // Animator.applyRootMotion = true;
        //     Animator.animatePhysics = true;
        //     UpdateDisabled = false;
        //     Animator.speed = 1;
        //     DamageDisabled = false;
        //     LockRotation = false;
        //     foreach (ParticleSystem trail in _trailParticles) {
        //         trail.Clear();
        //         trail.Stop();
        //     }
        // }
        Invoke(nameof(endDash), DashDuration);
    }

    private void endDash() {
        SetAnimationState(AnimationState.Locomotion);
        // CharacterController.SimpleMove(getGoalDirection() * MovementSpeed);

        Animator.speed = 1;
        DamageDisabled = false;
        LockRotation = false;
        foreach (ParticleSystem trail in _trailParticles) {
            trail.Clear();
            trail.Stop();
        }
    }

    // przeniesc do save systemu 

    public void OnEnable() {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
    }

    public void OnDisable() {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }


    #endregion

    #region Interaction Methods

    private void handleInteraction() {
        if(_queuedInteraction == null) return;
        if(UICanvas.CurrentUIMiddleState != UIMiddleState.NotVisible || UICanvas.CurrentUITopState != UITopState.NotVisible || UICanvas.CurrentUIBottomState != UIBottomState.HUD) {
            _queuedInteraction = null;
            return;
        }
        if(EventSystem.current.IsPointerOverGameObject()) return;


        interact(_queuedInteraction.Value);

        _queuedInteraction = null;
    }
    private void interact(InteractionType interactionType) {

        bool interacted = tryInteract(interactionType);
        
        if(interacted) return;
        if(CurrentWeapon == null) return;
        if(CurrentWeapon.ItemData == null) return;

        // Default to attacking if no interaction was commited
        
        if (CurrentAnimationState == AnimationState.Attack_ComboWindow) {
            _queuedRotation = GetMousePosition() - transform.position;
            LockRotation = false;
        } else if(_isAttacking) {
            return;
        }
        
        if (!LockRotation) {
            transform.LookAt(GetMousePosition());   
        }

        LockRotation = true;
        switch(interactionType) {
            case InteractionType.Primary:
                performLightAttack();
                break;
            case InteractionType.Secondary:
                performHeavyAttack();
                break;
        }
    }

    

    private bool tryInteract(InteractionType interactionType) {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _interactionMask)) {
            return false;
        }

        Transform objectHit = hit.transform;
        Debug.Log(objectHit.name);

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
        Animator.SetBool(_heavyAttackHash, false);
        Animator.SetTrigger(_lightAttackHash);
        AudioClip attackSound = Resources.Load("SFX/bron/atak4") as AudioClip;
        SoundFXManager.Instance.PlaySoundFXClip(attackSound, transform, 0.35f);
    }

    private void performHeavyAttack() {
        Animator.SetBool(_lightAttackHash, false);
        Animator.SetTrigger(_heavyAttackHash);
        AudioClip attackSound = Resources.Load("SFX/bron/atak2") as AudioClip;
        SoundFXManager.Instance.PlaySoundFXClip(attackSound, transform, 0.35f);
    }

    public void OnInventoryChanged() {
        UpdateEquipment();
        
    }
    public void UpdateEquipment(){
        WeaponHolder.UpdateWeapon(CurrentWeapon);
        Animator.SetInteger(_weaponTypeHash, (int) (CurrentWeapon?.ItemData?.WeaponType ?? WeaponType.None));

        if (CurrentWeapon?.ItemData != null) {
            Debug.Log(CurrentWeapon.ItemData.WeaponPrefab.WeaponTrait);
            SlashManager.SetSlashColor(CurrentWeapon.ItemData.WeaponPrefab.WeaponTrait);
        } 
        EventBus.InventoryItemChangedEvent.Invoke();
    }

    public Vector3 GetMousePosition() {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayerMask)) {
            Vector3 point = hit.point;
            point.y = transform.position.y;
            return point;
        } else {
            Debug.LogWarning("GetMousePosition: Raycast didnt hit ground");
            return Vector3.zero;
        }
    }

    #endregion

    #region Combat Methods

    public enum AnimationState {
        Locomotion,  
        Attack_Windup,
        Attack_Contact,
        Attack_ComboWindow,
        Attack_Recovery,
        Spell_Cast,
        Dash
    }

    public void SetAnimationState(AnimationState state) {
        exitAnimationState(CurrentAnimationState);
        enterAnimationState(state);
    }

    private void exitAnimationState(AnimationState state) {
        switch (state) {
            case AnimationState.Dash:
                _currentSpeed = MovementSpeed;
                break;

            case AnimationState.Locomotion:
                break;

            case AnimationState.Attack_Windup:
                break; 

            case AnimationState.Attack_Contact:
                break;

            case AnimationState.Attack_ComboWindow:
                _isAttacking = false;
                break;

            case AnimationState.Attack_Recovery:
                SlashManager.DisableSlash();
                _isAttacking = false;
                LockRotation = false;
                break;

            case AnimationState.Spell_Cast:
                UpdateDisabled = false;
                LockRotation = false;
                break;
        }
    }

    private void enterAnimationState(AnimationState state) {
        CurrentAnimationState = state;
        switch (state) {
            case AnimationState.Locomotion:
                WeaponHolder.DisableHitbox();
                break;

            case AnimationState.Attack_Windup:
                LockRotation = true;
                _isAttacking = true;
                _currentSpeed = 0;
                SlashManager.EnableSlash();
                WeaponHolder.EnableHitbox();
                // SlashGO.SetActive(true);

                break;

            case AnimationState.Attack_Contact:
                break;

            case AnimationState.Attack_ComboWindow:
                break;

            case AnimationState.Attack_Recovery:
                WeaponHolder.DisableHitbox();
                LockRotation = false;
                // LockRotation = true;
                break;

            case AnimationState.Spell_Cast:
                UpdateDisabled = true;
                LockRotation = true;
                break;
        }
    }

    public void OnAttackAnimationStart(AttackType attackType) {
        WeaponHolder.InitializeAttack(attackType);
        WeaponHolder.BeginAttack();
    }

    public void OnAttackAnimationEnd(AttackType _) {
        WeaponHolder.EndAttack();
    }

    #endregion

    #region Spell Methods

    void CastSpell(Spell spell) {
        if (spell == null) return;
        if (_queuedSpell != null) return;

        _queuedSpell = spell;

        spell.Cast();
        Animator.SetTrigger(_spellHash);

        if (!LockRotation) {
            transform.LookAt(GetMousePosition());   
        }
    }

    public void OnSpellCastReady() {
        Spell spell = _queuedSpell;

        _queuedSpell = null;

        if (!LockRotation) {
            transform.LookAt(GetMousePosition());   
        }

        spell.OnCastReady();
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
        if (_currentAmuletModifiers != null) {
            foreach(var modifier in _currentAmuletModifiers) {
                LivingEntity.ApplyIndefiniteModifier(modifier);
            }
        }

        // Ring modifiers
        if (_currentRingModifiers != null) {
            foreach(var modifier in _currentRingModifiers) {
                LivingEntity.ApplyIndefiniteModifier(modifier);
            }
        }

        Player.Instance.GetComponent<HumanoidInventory>().Clear();
        Player.Instance.GetComponent<HumanoidInventory>().OnInventoryChanged?.Invoke();
        Player.Instance.ConsumableItemOne = null;
        Player.Instance.ConsumableItemTwo = null;
        Player.Instance.SpellSlotOne = null;
        Player.Instance.SpellSlotTwo = null;
        Player.Instance.SpellSlotThree = null;
        Player.Instance.WeaponHolder.UpdateWeapon(null);
        Player.Instance.WeaponHolder.DisableHitbox();
        Player.Instance.WeaponHolder.EndAttack();
        Player.UICanvas.HUDCanvas.UpdateSpellSlots();
        Player.UICanvas.HUDCanvas.UpdateHealthBar();
        Player.UICanvas.HUDCanvas.UpdateManaBar();
        Player.UICanvas.HUDCanvas.OnUpdateConsumables();
        EventBus.InventoryItemChangedEvent?.Invoke();
        GameManager.Instance.ResetCards();
        GameManager.Instance.ResetInfluence();
        GameManager.Instance.ResetCardChoice();
        Player.Instance.GetComponent<HumanoidInventory>().AddItem(StarterWeapons[UnityEngine.Random.Range(0, StarterWeapons.Count)], 1, 1);
        EventBus.InventoryItemChangedEvent?.Invoke();

    }

    private void registerStats() {
        ModifierSystem.RegisterStat(ref VekhtarControl);
        ModifierSystem.RegisterStat(ref Armor);
        // ModifierSystem.RegisterStat(ref LightAttackDamage);
        ModifierSystem.RegisterStat(ref LightAttackSpeed);
        // ModifierSystem.RegisterStat(ref LightAttackRange);
        // ModifierSystem.RegisterStat(ref HeavyAttackDamage);
        ModifierSystem.RegisterStat(ref HeavyAttackSpeed);
        // ModifierSystem.RegisterStat(ref HeavyAttackRange);
        ModifierSystem.RegisterStat(ref MovementSpeed);
        ModifierSystem.RegisterStat(ref DashSpeedMultiplier);
        ModifierSystem.RegisterStat(ref DashCooldown);
        ModifierSystem.RegisterStat(ref DashDuration);
    }

    public void SetPlayerPosition(Vector3 position, float time = 0, float yRotation = 45) {
        UpdateDisabled = true;
        Animator.animatePhysics = false;
        Instance.gameObject.transform.position = position;
        Animator.animatePhysics = true;
        UpdateDisabled = false;
        Instance.gameObject.transform.DORotate(new Vector3(0, yRotation, 0), time);
    }
    public void PlayRespawnAnimation() {
        Player.Animator.animatePhysics = false;
        Player.Instance.UpdateDisabled = true;
        transform.position = StartPosition - Vector3.up * 3f;
        transform.rotation = Quaternion.Euler(0, 45, 0);
        float dissolve = 1f;
        DOTween.To(() => dissolve, x => dissolve = x, 0f, 2f).SetDelay(1f).OnUpdate(() => {
            _dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
        }).OnComplete(() => {
            Player.Instance.gameObject.transform.DOMoveY(-2, 0);
            Player.Animator.SetTrigger("rise");
            Player.Instance.gameObject.transform.DOComplete();

            Player.Instance.gameObject.transform.DOKill();

            dissolve = 0f;
            DOTween.To(() => dissolve, x => dissolve = x, 1f, 4f).SetDelay(1f).OnUpdate(() => {
                _dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
            });
            Player.Instance.gameObject.transform.DOMoveY(1, 2f).SetEase(Ease.OutQuint).OnComplete(() => {
                Player.Animator.SetTrigger("live");
                Player.Instance.UpdateDisabled = false;
                Player.Animator.animatePhysics = true;
                
            });
        });
   
    }

    public void ResetToDefault() {
        Instance.LockRotation = false;
        Instance.UpdateDisabled = false;
        Instance.DamageDisabled = false;
        Instance.HasPlayerDied = false;
        Instance._isAttacking = false;
        Instance.CurrentAnimationState = AnimationState.Locomotion;
        Instance.SlashManager.DisableSlash();
        Instance.UpdateEquipment();
    }

    #endregion
    #region Save System
    public void Save(ref PlayerSaveData data){
        data.EvolutionPoints = EvolutionPoints;
        data.SelectedEvolutions = SelectedEvolutions;
        data.ConsumableItemOne = ConsumableItemOne;
        data.ConsumableItemTwo = ConsumableItemTwo;
    }
    public void Load(PlayerSaveData data){
        EvolutionPoints = data.EvolutionPoints;
        foreach(EvoUI evolution in data.SelectedEvolutions){
            evolution.AddEvolution();   
        }
        ConsumableItemOne = data.ConsumableItemOne;
        ConsumableItemTwo = data.ConsumableItemTwo;
    }
    [Serializable]
    public struct PlayerSaveData{
        public List<EvoUI> SelectedEvolutions;
        public int EvolutionPoints;
        public InventoryItem<ConsumableItemData> ConsumableItemOne;
        public InventoryItem<ConsumableItemData> ConsumableItemTwo;
    }
    #endregion
}
