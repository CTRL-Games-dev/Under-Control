using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Behavior;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using FMOD.Studio;

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
    public Stat VekhtarControl = new DynamicStat(StatType.VEKTHAR_CONTROL);
    public DynamicStat Armor => LivingEntity.Armor;

    // public Stat LightAttackDamage = new Stat(StatType.LIGHT_ATTACK_DAMAGE, 10f);
    public Stat LightAttackSpeed = new Stat(StatType.LIGHT_ATTACK_SPEED);
    // public Stat LightAttackRange = new Stat(StatType.LIGHT_ATTACK_RANGE, 1f);

    // public Stat HeavyAttackDamage = new Stat(StatType.HEAVY_ATTACK_DAMAGE, 20f);
    public Stat HeavyAttackSpeed = new Stat(StatType.HEAVY_ATTACK_SPEED);
    // public Stat HeavyAttackRange = new Stat(StatType.HEAVY_ATTACK_RANGE, 1f);

    public Stat MovementSpeed = new Stat(StatType.MOVEMENT_SPEED);

    public Stat DashSpeedMultiplier = new Stat(StatType.DASH_SPEED_MULTIPLIER);
    public Stat DashCooldown = new Stat(StatType.DASH_COOLDOWN);
    public Stat DashDuration = new Stat(StatType.DASH_COOLDOWN);

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

    //Audio
    private EventInstance _PlayerWalkSound;

    private int _evolutionPoints = 0;
    public int EvolutionPoints {
        get{ return _evolutionPoints; }
        set {
            _evolutionPoints = value;
            UICanvas.InventoryCanvas.ChangeEvoPoints();
        }
    }

    private AttackType? _attackType;

    public List<EvoUI> SelectedEvolutions;

    [Header("Fishing")]
    public bool CanFish = false;
    public bool FishCatchWindow = false;
    [SerializeField] private WeaponItemData _fishingRod;
    public Transform FishingBone;
    public float FishingForce = 1;
    public int AvailableFish = 0;
    [SerializeField] private GameObject _bobberPrefab;
    [SerializeField] private GameObject _currentBobber;

    [Header("Spells")]
    [SerializeField]
    private SpellData _spellDataOne; 
    public Spell SpellSlotOne {
        get => _spellDataOne.Spell;
        set {
            _spellDataOne.Spell = value;
            _spellDataOne.Cooldown = value == null ? null : new Cooldown(value.CooldownTime); 
            UICanvas.HUDCanvas.UpdateSpellSlots();
        }
    }

    [SerializeField]
    private SpellData _spellDataTwo;
    public Spell SpellSlotTwo {
        get => _spellDataTwo.Spell;
        set {
            _spellDataTwo.Spell = value;
            _spellDataTwo.Cooldown = value == null ? null : new Cooldown(value.CooldownTime);
            UICanvas.HUDCanvas.UpdateSpellSlots();
        }
    }

    [SerializeField]
    private SpellData _spellDataThree;
    public Spell SpellSlotThree {
        get => _spellDataThree.Spell;
        set {
            _spellDataThree.Spell = value;
            _spellDataThree.Cooldown = value == null ? null : new Cooldown(value.CooldownTime);
            UICanvas.HUDCanvas.UpdateSpellSlots();
        }
    }

    private Spell _queuedSpell = null;

    [Header("Consumable")]
    public InventoryItem<ConsumableItemData> ConsumableItemOne = null;
    public InventoryItem<ConsumableItemData> ConsumableItemTwo = null;
    public Cooldown ConsumableCooldown = new Cooldown(0.5f);

    [Header("Weapon")]
    public PlayerWeaponHolder WeaponHolder;
    public InventoryItem<WeaponItemData> CurrentWeapon { get => Inventory.Weapon; }
    public InventoryItem<ArmorItemData> CurrentArmor { get => Inventory.Armor; }

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
    public UnityEvent FishCaughtEvent;
    public float CameraDistance { get => CinemachinePositionComposer.CameraDistance; set => CinemachinePositionComposer.CameraDistance = value; }

    // State
    private Vector2 _movementInputVector = Vector2.zero;
    private InteractionType? _queuedInteraction;
    private Cooldown _dashCooldown = new Cooldown(0);
    public bool InvertedControls = false;

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
    [SerializeField] public GameObject FBXModel;
    public GameObject  FishingRod;
    [SerializeField] private SkinnedMeshRenderer _pauldronRenderer;
    [SerializeField] private Material _defaultPauldronMaterial;

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
    private Knockback _knockback;
    private LayerMask _interactionMask;
    public FaceAnimator FaceAnimator;
    public AnimationState CurrentAnimationState = AnimationState.Locomotion;
    public InputActionAsset actions;
    private Vector3 _queuedRotation = Vector3.zero;
    public List<WeaponItemData> StarterWeapons = new List<WeaponItemData>();

    public bool FullDamageDisable = false;

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
        _knockback = GetComponent<Knockback>();

        Instance = this;

        LivingEntity.OnDeath.AddListener(onDeath);
        LivingEntity.OnStunned.AddListener(onStunned);
        LivingEntity.OnDamageTaken.AddListener(onDamageTaken);
        EventBus.ItemPlacedEvent.AddListener(UpdateEquipment);
        
        CameraDistance = MinCameraDistance;

        registerStats();

        StartPosition = transform.position;

        OnEvolutionSelected.AddListener(x => ApplyEvolution(x));

 
        _interactionMask |= 1 << LayerMask.NameToLayer("Interactable");

        // LoadKeybinds();
    }

    void Start() {
        _PlayerWalkSound = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PlayerWalkSound);
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
        


        ResetRun();
    }

void Update() {
    if (UpdateDisabled) return;

    handleInteraction();
    handleRotation();

    _currentSpeed = Mathf.MoveTowards(_currentSpeed, getGoalSpeed(), getSpeedChange() * Time.deltaTime);
    CharacterController.SimpleMove(getGoalDirection() * _currentSpeed);

    if (_PlayerWalkSound.isValid()) {
        _PlayerWalkSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

        FMOD.Studio.PLAYBACK_STATE playbackState;
        _PlayerWalkSound.getPlaybackState(out playbackState);

        if (_currentSpeed > 0.1f) {
            if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED || playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPING) {
                _PlayerWalkSound.start();
            }
        } else {
            if (playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING) {
                _PlayerWalkSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}

    void FixedUpdate() {
        // Magiczna liczba to predkosc animacji biegu
        Animator.SetFloat(_speedHash, _currentSpeed / MovementSpeed);
        Animator.SetFloat(_lightAttackSpeedHash, LightAttackSpeed);
        Animator.SetFloat(_heavyAttackSpeedHash, HeavyAttackSpeed);
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
                // if (_queuedRotation == Vector3.zero) return transform.forward;
                // Vector3 dir = _queuedRotation;
                // _queuedRotation = Vector3.zero;
                // return dir.normalized;
            case AnimationState.Attack_Contact:
            case AnimationState.Attack_ComboWindow:
                // return transform.forward;

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
        UICanvas.HUDCanvas.HideBossBar();   
        _knockback.Reset();
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
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.playerStunnedIndicator, transform.position);
        CameraManager.ShakeCamera(2, duration);
    }
    
    private void onDamageTaken(DamageTakenEventData _) {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerHitIndicator, transform.position);
        FaceAnimator.StartAnimation("HURT", 0.3f);
        CameraManager.ShakeCamera(0.7f, 0.1f);

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
        useConsumable(ref ConsumableItemOne, 1);
    }

    void OnUseConsumableTwo(InputValue value) {
        useConsumable(ref ConsumableItemTwo, 2);
    }
    private void useConsumable(ref InventoryItem<ConsumableItemData> consumable, int slotIndex) {
        if(consumable == null) return;
        if(!ConsumableCooldown.Execute()) return;
        if(consumable.Amount <= 0) return;

        ConsumableItemData c =  consumable.ItemData as ConsumableItemData;
        
        if(c == null) return;
        if(!c.Consume(LivingEntity)) return;
        consumable.Amount--;
        if (consumable.Amount <= 0) {
            consumable = null;
        }

        UICanvas.HUDCanvas.UseConsumable(slotIndex);
        UpdateConsumablesEvent?.Invoke();
    }

    void OnMove(InputValue value) {
        _movementInputVector = value.Get<Vector2>();
        _movementInputVector *= (InvertedControls ? -1 : 1);
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
        Animator.SetBool(_lightAttackHash, false);
        Animator.SetBool(_heavyAttackHash, false);

        foreach (ParticleSystem trail in _trailParticles) { trail.Play(); }
        Debug.Log(transform.position);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerDashSound, transform.position);
        CurrentAnimationState = AnimationState.Dash;

        
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
    if (_PlayerWalkSound.isValid())
{
        _PlayerWalkSound.release();
    }
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
        Debug.Log($"Interacting with {interactionType}");

        bool interacted = tryInteract(interactionType);
        
        if(interacted) return;
        if(CurrentWeapon == null) return;
        if(CurrentWeapon.ItemData == null) return;

        //fishing interaction
        if(CurrentWeapon.ItemData == _fishingRod){
            if(!CanFish) return;
            if (!LockRotation) transform.LookAt(GetMousePosition());
            Animator.SetTrigger("castRod");
            // tryFish();
            return;
        }

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
                _attackType = AttackType.LIGHT;
                break;
            case InteractionType.Secondary:
                performHeavyAttack();
                _attackType = AttackType.HEAVY;
                break;
        }
    }

    private bool tryFish(){
        if(_currentBobber == null){
            _currentBobber = Instantiate(_bobberPrefab,FishingBone.position,transform.rotation,null);
            _currentBobber.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0.6f, 1) * FishingForce, ForceMode.Impulse);
            Player.Instance.InputDisabled = true;
            return true;
        }
        Animator.SetTrigger("catchFish");
        Player.Instance.InputDisabled = false;
        if(FishCatchWindow){
            ConsumableItemData caughtFish = GameManager.Instance.CatchableFish[UnityEngine.Random.Range(0, GameManager.Instance.CatchableFish.Count())] as ConsumableItemData;
            Inventory.AddItem(caughtFish,1,1);
            UICanvas.PickupItemNotify(caughtFish, 1);
            AvailableFish -= 1;
            FishCaughtEvent?.Invoke();
            FishCatchWindow = false;

        }
        Destroy(_currentBobber);

        return true;
    }

    private bool tryInteract(InteractionType interactionType) {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _interactionMask)) {
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
        Animator.SetBool(_heavyAttackHash, false);
        Animator.SetTrigger(_lightAttackHash);
    }

    private void performHeavyAttack() {
        Animator.SetBool(_lightAttackHash, false);
        Animator.SetTrigger(_heavyAttackHash);
    }

    public void OnInventoryChanged() {
        UpdateEquipment();
    }
    public void UpdateEquipment(){
        WeaponHolder.UpdateWeapon(CurrentWeapon);
        Animator.SetInteger(_weaponTypeHash, (int) (CurrentWeapon?.ItemData?.WeaponType ?? WeaponType.None));

        if (CurrentWeapon?.ItemData != null) {
            SlashManager.SetSlashColor(CurrentWeapon.ItemData.WeaponPrefab.WeaponTrait);
        } 
        EventBus.InventoryItemChangedEvent.Invoke();
        if(CurrentArmor?.ItemData != null){
            Armor.Set(CurrentArmor.ItemData.Armor);
            _pauldronRenderer.material = CurrentArmor.ItemData.Material;
        }
        else{
            Armor.Set(0);
            _pauldronRenderer.material = _defaultPauldronMaterial;
        }
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
                WeaponHolder.DisableHitbox();
                SlashManager.DisableSlash();
                WeaponHolder.EndAttack();
                break;

            case AnimationState.Attack_Recovery:
                WeaponHolder.DisableHitbox();
                SlashManager.DisableSlash();
                WeaponHolder.EndAttack();
                _isAttacking = false;
                _attackType = null;
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
                bool isPlayerAttackPaid = false;

                if(_attackType == AttackType.HEAVY) {
                    float manaPrice = WeaponHolder.GetManaCost();
                    if(manaPrice <= Mana) {
                        LivingEntity.Mana -= manaPrice;
                        isPlayerAttackPaid = true;
                    }
                }

                WeaponHolder.InitializeAttack(_attackType.Value, isPlayerAttackPaid);
                WeaponHolder.BeginAttack();
                AudioManager.Instance.PlayAttackSound(FMODEvents.Instance.PlayerAttack, this.transform.position, CurrentWeapon.ItemData.WeaponType);
                LockRotation = true;
                _isAttacking = true;
                _currentSpeed = 0;
                SlashManager.EnableSlash();
                // SlashGO.SetActive(true);

                break;

            case AnimationState.Attack_Contact:
                AudioManager.Instance.PlayAttackSound(FMODEvents.Instance.AttackContact, this.transform.position, CurrentWeapon.ItemData.WeaponType);
                WeaponHolder.EnableHitbox();
                break;

            case AnimationState.Attack_ComboWindow:
                break;

            case AnimationState.Attack_Recovery:
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

    }

    public void OnAttackAnimationEnd(AttackType _) {
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

        GetComponent<HumanoidInventory>().Clear();
        GetComponent<HumanoidInventory>().OnInventoryChanged?.Invoke();
        ConsumableItemOne = null;
        ConsumableItemTwo = null;
        SpellSlotOne = null;
        SpellSlotTwo = null;
        SpellSlotThree = null;
        WeaponHolder.UpdateWeapon(null);
        WeaponHolder.DisableHitbox();
        WeaponHolder.EndAttack();
        UICanvas.HUDCanvas.UpdateSpellSlots();
        UICanvas.HUDCanvas.UpdateHealthBar();
        UICanvas.HUDCanvas.UpdateManaBar();
        UICanvas.HUDCanvas.OnUpdateConsumables();
        UICanvas.ChooseCanvas.ResetCardUI();
        // GetComponent<HumanoidInventory>().AddItem(StarterWeapons[UnityEngine.Random.Range(0, StarterWeapons.Count)], 1, 1);
        GameManager.Instance.ResetCards();
        GameManager.Instance.ResetInfluence();
        GameManager.Instance.ResetCardChoice();
        CanFish = false;
        // GetComponent<HumanoidInventory>().AddItem(StarterWeapons[UnityEngine.Random.Range(0, StarterWeapons.Count)], 1, 1);
        GetComponent<HumanoidInventory>().OnInventoryChanged?.Invoke();
        EventBus.InventoryItemChangedEvent?.Invoke();
        GameManager.Instance.LevelDepth = 0;

        Instance.UpdateEquipment();
        LivingEntity.Health = LivingEntity.MaxHealth;
        LivingEntity.Mana = LivingEntity.MaxMana;
    }

    private void registerStats() {
        ModifierSystem.RegisterStat(ref VekhtarControl);
        //ModifierSystem.RegisterStat(ref Armor);
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
    
    public void ApplyEvolution(EvoUI evoUI) {
        GameManager.Instance.RandomCardCount = 3;
        
        switch (evoUI.ElementalType) {
            case ElementalType.Fire:
            case ElementalType.Ice:
                foreach (Modifier modifier in evoUI.GetModifiers()) {
                    LivingEntity.ApplyIndefiniteModifier(modifier);
                }
                break;
            case ElementalType.Earth:
                GameManager.Instance.RandomCardCount++;
                break;
        }
        LivingEntity.Health = LivingEntity.MaxHealth;
        LivingEntity.Mana = LivingEntity.MaxMana;
         
    }
    public bool BuyFishingRod(Transform t){
        if (Coins < 150) return false;
        Coins -= 150;

        Inventory.AddItem(_fishingRod, 1, 1);

        GetComponent<HumanoidInventory>().OnInventoryChanged?.Invoke();

        return true;
    }

    public void EquipFishingRod(bool val) {
        FishingRod.SetActive(val);
    }

    public void SetPlayerPosition(Vector3 position, float time = 0.1f, float yRotation = 45) {
        Instance.gameObject.SetActive(true);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        StartCoroutine(setPlayerPositionCoroutine(position, time));
    }

    private IEnumerator setPlayerPositionCoroutine(Vector3 position, float time) {
        UpdateDisabled = true;
        Animator.animatePhysics = false;
        Animator.speed = 0;
        gameObject.transform.position = position;
        yield return new WaitForSeconds(time);
        Animator.animatePhysics = true;
        Animator.speed = 1;
        UpdateDisabled = false;
    }



    public void PlayRespawnAnimation() {
        AudioManager.Instance.setMusicArea(MusicArea.HUB);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.RespawnSound, this.transform.position);
        Animator.animatePhysics = false;
        UpdateDisabled = true;
        transform.position = StartPosition - Vector3.up * 3f;
        transform.rotation = Quaternion.Euler(0, 45, 0);
        float dissolve = 1f;
        DOTween.To(() => dissolve, x => dissolve = x, 0f, 2f).SetDelay(1f).OnUpdate(() => {
            _dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
        }).OnComplete(() => {
            gameObject.transform.DOMoveY(-2, 0);
            Animator.SetTrigger("rise");
            gameObject.transform.DOComplete();

            gameObject.transform.DOKill();

            dissolve = 0f;
            DOTween.To(() => dissolve, x => dissolve = x, 1f, 4f).SetDelay(1f).OnUpdate(() => {
                _dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
            });
            gameObject.transform.DOMoveY(1, 2f).SetEase(Ease.OutQuint).OnComplete(() => {
                Animator.SetTrigger("live");
                UpdateDisabled = false;
                Animator.animatePhysics = true;
                UICanvas.ChangeUIBottomState(UIBottomState.HUD);
                
            });
        });
   
    }

    public void ResetToDefault() {
        LockRotation = false;
        UpdateDisabled = false;
        DamageDisabled = false;
        HasPlayerDied = false;
        _isAttacking = false;
        CurrentAnimationState = AnimationState.Locomotion;
        SlashManager.DisableSlash();
        UpdateEquipment();
    }
    
    public void TryCatchFish() {
        tryFish();
    }

    #endregion
    #region Save System
    public void Save(ref PlayerSaveData data){
        data.EvolutionPoints = EvolutionPoints;
        data.SelectedEvolutions = SelectedEvolutions;
        data.ConsumableItemOne = ConsumableItemOne;
        data.ConsumableItemTwo = ConsumableItemTwo;
        data.Health = Health;
        data.Mana = Mana;
    }
    public void Load(PlayerSaveData data){
        EvolutionPoints = data.EvolutionPoints;
        foreach(EvoUI evolution in data.SelectedEvolutions){
            evolution.AddEvolution();   
        }
        ConsumableItemOne = data.ConsumableItemOne;
        ConsumableItemTwo = data.ConsumableItemTwo;
        LivingEntity.Health = data.Health;
        LivingEntity.Mana = data.Mana;
    }
    [Serializable]
    public struct PlayerSaveData{
        public float Health;
        public float Mana;
        public List<EvoUI> SelectedEvolutions;
        public int EvolutionPoints;
        public InventoryItem<ConsumableItemData> ConsumableItemOne;
        public InventoryItem<ConsumableItemData> ConsumableItemTwo;
    }
    #endregion
    
}
