using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ModifierSystem))]
[RequireComponent(typeof(EntityInventory))]
[RequireComponent(typeof(HitFlashAnimator))]
public class LivingEntity : MonoBehaviour {
    private struct EffectData {
        public Effect Effect;
        public float Expiration;
    }

    [Header("Properties")]
    public string DisplayName;
    public Guild Guild;
    public bool DropItemsOnDeath = true;
    public bool DestroyOnDeath = true;
    public bool IsInvisible = false;

    public string DebugName => $"{DisplayName} ({Guild.Name} {gameObject.name})";

    [Header("Stats")]
    [SerializeField] 
    private float _startingHealth = 100;
    [SerializeField]
    private float _startingMana = 100f;
    private float _health = 0;
    public float Health {
        get => _health;
        set {
            _health = value;
            if (_isPlayer) Player.UICanvas.HUDCanvas.UpdateHealthBar();
        }
    }
    private float _mana = 100f;
    public float Mana {
        get => _mana;
        set {
            _mana = value;
            if (_isPlayer) Player.UICanvas.HUDCanvas.UpdateManaBar();
        }
    }
   
    public Stat MaxHealth = new Stat(StatType.MAX_HEALTH, 100);
    public Stat Armor = new Stat(StatType.ARMOR, 0);
    public Stat ElementalArmor = new Stat(StatType.ELEMENTAL_ARMOR, 0);
    public Stat MovementSpeed = new Stat(StatType.MOVEMENT_SPEED, 1);
    public Stat MaxMana = new Stat(StatType.MAX_MANA, 100f);

    [Header("Events")]
    public UnityEvent OnDeath;
    public UnityEvent<DamageTakenEventData> OnDamageTaken;
    public UnityEvent<float> OnStunned; // float - stun duration

    // State
    private List<EffectData> _activeEffects = new List<EffectData>();

    private readonly int _hurtHash = Animator.StringToHash("hurt");

    // References
    public ModifierSystem ModifierSystem { get; private set; }
    public EntityInventory Inventory { get; private set; }
    public HitFlashAnimator HitFlashAnimator { get; private set; }

    private bool _isPlayer = false;

    void Awake() {
        ModifierSystem = GetComponent<ModifierSystem>();
        Inventory = GetComponent<EntityInventory>();
        HitFlashAnimator = GetComponent<HitFlashAnimator>();

        ModifierSystem.RegisterStat(ref MaxHealth);
        ModifierSystem.RegisterStat(ref Armor);
        ModifierSystem.RegisterStat(ref ElementalArmor);
        ModifierSystem.RegisterStat(ref MovementSpeed);
        ModifierSystem.RegisterStat(ref MaxMana);

        _isPlayer = gameObject.GetComponent<Player>() != null;
        _health = _startingHealth;
        _mana = _startingMana;
    }

    void Update() {
        recheckEffects();
    }

    public void DropItem(InventoryItem item) {
        dropItem(item.ItemData, item.Amount);
        Inventory.RemoveInventoryItem(item);
    }

    // Spawns item at torso level and throws item on the ground
    private void dropItem(ItemData itemData, int amount) {
        ItemEntity.SpawnThrownRelative(itemData, amount, transform.position + new Vector3(0, 1.2f, 0), transform.rotation, Vector3.forward * 2);
    }

    public void Attack(Damage damage, LivingEntity target) {
        target.takeDamage(damage, this);
    }

    private void takeDamage(Damage damage, LivingEntity source = null) {
        if (source.gameObject.CompareTag("Player")) {
            // StartCoroutine(nameof(slowDown));
            CameraShake.Instance.Shake(2, 0.1f);
        }

        if (gameObject.CompareTag("Boar")) {
            gameObject.GetComponent<Animator>()?.SetTrigger(_hurtHash);
        }


        // Check if entity is dead
        if(Health == 0) {
            return;
        }

        float desiredDamageAmount = damage.Value;
        // TODO: Calculate damage based on damage type, current entity modifiers, spells and what not

        float resistance = getDamageResistance(damage.Type);
        if(resistance < 0) {
            Debug.LogWarning($"Resistance for damage type {damage.Type} is negative. Resistance is floored to 0. Resistance is {resistance}");
            resistance = 0;
        } else if (resistance > 1) {
            Debug.LogWarning($"Resistance for damage type {damage.Type} is greater than 1. Resistance is floored to 1. Resistance is {resistance}");
            resistance = 1;
        }

        desiredDamageAmount *= 1 - resistance;

        float actualDamageAmount = desiredDamageAmount;
        if(actualDamageAmount > Health) {
            actualDamageAmount = Health;
        }

        Health -= actualDamageAmount; 

        OnDamageTaken.Invoke(new DamageTakenEventData {
            Damage = damage,
            DesiredDamageAmount = desiredDamageAmount,
            ActualDamageAmount = actualDamageAmount,
            Attacker = source,
            Victim = this
        });

        HitFlashAnimator.Flash();

        if (Health == 0) {
            // Drop items
            if(DropItemsOnDeath) {
                // Drop common slots
                List<InventoryItem> items = new List<InventoryItem>(Inventory.GetItems());
                foreach(InventoryItem item in items) {
                    dropItem(item.ItemData, item.Amount);
                    Inventory.RemoveInventoryItem(item);
                }

                // Drop equipment
                if(Inventory is HumanoidInventory humanoidInventory) {
                    if(humanoidInventory.Armor != null) {
                        dropItem(humanoidInventory.Armor, 1);
                        humanoidInventory.Armor = null;
                    }

                    if(humanoidInventory.Amulet != null) {
                        dropItem(humanoidInventory.Amulet, 1);
                        humanoidInventory.Amulet = null;
                    }

                    if(humanoidInventory.Weapon != null) {
                        dropItem(humanoidInventory.Weapon, 1);
                        humanoidInventory.Weapon = null;
                    }
                }
            }

            OnDeath.Invoke();

            if (DestroyOnDeath) Destroy(gameObject);
        }
    }

    private float getDamageResistance(DamageType damageType) {
        if(Inventory is not HumanoidInventory humanoidInventory) return 0;

        return humanoidInventory.Armor?.DamageResistances.Where(x => x.DamageType == damageType).Sum(x => x.Resistance) ?? 0;
    }

    #region Effects

    public void ApplyEffect(Effect effect) {
        _activeEffects.Add(new EffectData {
            Effect = effect,
            Expiration = Time.time + effect.Duration
        });

        if(effect.Modifiers == null) {
            return;
        }

        for(int i = 0; i < effect.Modifiers.Length; i++) {
            var modifier = effect.Modifiers[i];
            ModifierSystem.ApplyTemporaryModifier(modifier, effect.Duration);
        }
    }

    public void RemoveEffect(Effect effect) {
        for(int i = 0; i < _activeEffects.Count; i++) {
            if(!_activeEffects[i].Effect.Equals(effect)) {
                continue;
            }

            for(int j = 0; j < effect.Modifiers.Length; j++) {
                var modifier = effect.Modifiers[j];
                ModifierSystem.RemoveModifier(modifier);
            }

            _activeEffects.RemoveAt(i);
        }
    }

    // public void RemoveAllEffectsLike(Effect effect) {
    //     for(int i = 0; i < activeEffects.Count; i++) {
    //         if(!activeEffects[i].effect.Equals(effect)) {
    //             continue;
    //         }

    //         var effect = activeEffects[i].effect;

    //         for(int j = 0; j < effect.modifiers.Length; j++) {
    //             var modifier = effect.modifiers[j];
    //             modifierSystem.RemoveModifier(modifier);
    //         }
            
    //         activeEffects.RemoveAt(i);
    //     }
    // }

    private void recheckEffects() {
        _activeEffects.RemoveAll(x => x.Expiration < Time.time);
    }

    #endregion

    #region Modifiers

    public void ApplyIndefiniteModifier(Modifier modifier) {
        ModifierSystem.ApplyIndefiniteModifier(modifier);
    }

    public void ApplyTemporaryModifier(Modifier modifier, float duration) {
        ModifierSystem.ApplyTemporaryModifier(modifier, duration);
    }

    public void RemoveModifier(Modifier modifier) {
        ModifierSystem.RemoveModifier(modifier);
    }

    #endregion
}