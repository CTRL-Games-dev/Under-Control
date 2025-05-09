using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ModifierSystem))]
[RequireComponent(typeof(EntityInventory))]
[RequireComponent(typeof(HitFlashAnimator))]
[RequireComponent(typeof(TintAnimator))]
public class LivingEntity : MonoBehaviour {
    public struct EffectData {
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
    public float StartingHealth = 100;
    public float StartingMana = 100f;

    [SerializeField]
    private float _health = 0;
    public float Health {
        get => _health;
        set {
            _health = value;
            if (_isPlayer) Player.UICanvas.HUDCanvas.UpdateHealthBar();
        }
    }
   
    [SerializeField]
    private float _mana = 100f;
    public float Mana {
        get => _mana;
        set {
            _mana = value;
            if (_isPlayer) Player.UICanvas.HUDCanvas.UpdateManaBar();
        }
    }
   
    public Stat MaxHealth = new Stat(StatType.MAX_HEALTH);
    public Stat Armor = new Stat(StatType.ARMOR);
    public Stat MovementSpeed = new Stat(StatType.MOVEMENT_SPEED);
    public Stat MaxMana = new Stat(StatType.MAX_MANA);

    [Header("Sounds")]
    public AudioClip OnDeathSound;
    public AudioClip OnDamageSound;
    public AudioClip OnAttack;
    public AudioClip[] IdleSounds;
    

    [Header("Events")]
    public UnityEvent OnDeath;
    public UnityEvent<DamageTakenEventData> OnDamageTaken;
    public UnityEvent<float> OnStunned; // float - stun duration

    // State
    private List<EffectData> _activeEffects = new List<EffectData>();
    public List<EffectData> ActiveEffects {
        get { return _activeEffects; }
        private set { _activeEffects = value; }
    }

    public bool HasDied = false;

    private readonly int _hurtHash = Animator.StringToHash("hurt");
    private readonly int _movementSpeedHash = Animator.StringToHash("movement_speed");

    // References
    public ModifierSystem ModifierSystem { get; private set; }
    public EntityInventory Inventory { get; private set; }
    public HitFlashAnimator HitFlashAnimator { get; private set; }
    public TintAnimator TintAnimator { get; private set; }
    private Animator _animator;

    public bool _isPlayer = false;

    void Awake() {
        _animator = GetComponent<Animator>();
        ModifierSystem = GetComponent<ModifierSystem>();
        Inventory = GetComponent<EntityInventory>();
        HitFlashAnimator = GetComponent<HitFlashAnimator>();
        TintAnimator = GetComponent<TintAnimator>();

        ModifierSystem.RegisterStat(ref MaxHealth);
        ModifierSystem.RegisterStat(ref Armor);
        ModifierSystem.RegisterStat(ref MovementSpeed);
        ModifierSystem.RegisterStat(ref MaxMana);
 
        _isPlayer = gameObject.GetComponent<Player>() != null;
        _health = StartingHealth;
        _mana = StartingMana;
        

        if (_isPlayer) {
            MaxHealth.OnValueChanged.AddListener(() => Player.UICanvas.HUDCanvas.UpdateHealthBar());
            MaxMana.OnValueChanged.AddListener(() => Player.UICanvas.HUDCanvas.UpdateManaBar());
        }
    }

    void Update() {
        recheckEffects();
        _animator.SetFloat(_movementSpeedHash, MovementSpeed);
        if (Health <= 0) {
            Die();
        }
    }

    public void DropItem(InventoryItem item) {
        dropItem(item.ItemData, item.Amount, item.PowerScale);
        Inventory.RemoveInventoryItem(item);
    }

    // Spawns item at torso level and throws item on the ground
    private void dropItem(ItemData itemData, int amount, float powerScale) {
        ItemEntity.SpawnThrownRelative(itemData, amount, transform.position + new Vector3(0, 1.2f, 0), powerScale, transform.rotation, Vector3.forward * 2);
    }

    private void dropItem(InventoryItem inventoryItem) {
        dropItem(inventoryItem.ItemData, inventoryItem.Amount, inventoryItem.PowerScale);
    }

    public void Attack(Damage damage, LivingEntity target) {
        target.TakeDamage(damage, this);
    }

    public void TakeDamage(Damage damage, LivingEntity source = null) {
        if (_isPlayer) {
            if (Player.Instance.DamageDisabled) {
                return;
            }
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
            Die();
        }
    }
    
    public void Die() {
        if (HasDied) return;
        HasDied = true;

        // Drop items
        if(DropItemsOnDeath) {
            // Drop common slots
            List<InventoryItem> items = new List<InventoryItem>(Inventory.GetItems());
            foreach(InventoryItem item in items) {
                dropItem(item);
                Inventory.RemoveInventoryItem(item);
            }

            // Drop equipment
            if(Inventory is HumanoidInventory humanoidInventory) {
                if(humanoidInventory.Armor != null) {
                    dropItem(humanoidInventory.Armor);
                    humanoidInventory.Armor = null;
                }

                if(humanoidInventory.Amulet != null) {
                    dropItem(humanoidInventory.Amulet);
                    humanoidInventory.Amulet = null;
                }

                if(humanoidInventory.Weapon != null) {
                    dropItem(humanoidInventory.Weapon);
                    humanoidInventory.Weapon = null;
                }
            }
        } else {
            AudioClip hitSound = Resources.Load("SFX/uderzenie") as AudioClip; //hitsound
            SoundFXManager.Instance.PlaySoundFXClip(hitSound, transform, 0.7f);
        }

        if(OnDeathSound!=null) SoundFXManager.Instance.PlaySoundFXClip(OnDeathSound, transform, 0.4f);

        OnDeath?.Invoke();

        if (DestroyOnDeath) Destroy(gameObject);
    }

    private float getDamageResistance(DamageType damageType) {
        if(Inventory is not HumanoidInventory humanoidInventory) return 0;
        var armor = humanoidInventory.Armor;
        if(armor == null) return 0;

        float resistanceValue = armor.ItemData?.DamageResistances.Where(x => x.DamageType == damageType).Sum(x => x.Resistance * armor.PowerScale) ?? 0;
        
        resistanceValue = resistanceValue < 0 ? 0 : resistanceValue;
        resistanceValue = resistanceValue > 90 ? 90 : resistanceValue;
        return resistanceValue;
    }

    #region Effects

    public void ApplyEffect(Effect effect) {
        ActiveEffects.Add(new EffectData {
            Effect = effect,
            Expiration = Time.time + effect.Duration
        });

        effect.OnApply(this);

        if(effect.Modifiers == null) {
            return;
        }

        for(int i = 0; i < effect.Modifiers.Length; i++) {
            var modifier = effect.Modifiers[i];
            ModifierSystem.ApplyTemporaryModifier(modifier, effect.Duration);
        }
    }

    public void RemoveEffect(Effect effect) {
        for(int i = 0; i < ActiveEffects.Count; i++) {
            if(!ActiveEffects[i].Effect.Equals(effect)) {
                continue;
            }

            for(int j = 0; j < effect.Modifiers.Length; j++) {
                var modifier = effect.Modifiers[j];
                ModifierSystem.RemoveModifier(modifier);
            }

            effect.OnRemove(this);

            ActiveEffects.RemoveAt(i);
        }
    }

    public List<EffectData> GetActiveEffects() {
        return ActiveEffects;
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
        ActiveEffects.RemoveAll(x => {
            if(x.Expiration < Time.time) {
                x.Effect.OnRemove(this);
                return true;
            }

            return false;
        });
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