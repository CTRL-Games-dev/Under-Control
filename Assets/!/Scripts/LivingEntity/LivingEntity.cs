using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ModifierSystem))]
[RequireComponent(typeof(EntityInventory))]
public class LivingEntity : MonoBehaviour
{
    private struct EffectData {
        public Effect Effect;
        public float Expiration;
    }

    [Header("Properties")]
    public string DisplayName;
    public Guild Guild;
    public bool DropItemsOnDeath = true;
    public float TimeToRegenAfterDamage = 2;
    public string DebugName => $"{DisplayName} ({Guild.Name}. {gameObject.name})";

    public int Exp = 0;
    public float Level { get => 1 + Exp / 100f; }

    [Range(0, 2)]
    public int DroppedExpMultiplier = 1;

    [Header("Stats")]
    public DynamicStat Health = new DynamicStat(StatType.HEALTH, 100);
    public Stat MaxHealth = new Stat(StatType.MAX_HEALTH, 100);
    public Stat RegenRate = new Stat(StatType.REGEN_RATE, 1);
    public Stat Armor = new Stat(StatType.ARMOR, 0);
    public Stat ElementalArmor = new Stat(StatType.ELEMENTAL_ARMOR, 0);
    public Stat MovementSpeed = new Stat(StatType.MOVEMENT_SPEED, 1);

    [Header("Events")]
    public UnityEvent OnDeath;
    public UnityEvent<DamageTakenEventData> OnDamageTaken;

    // State
    private float _lastDamageTime = 0;
    private List<EffectData> _activeEffects = new List<EffectData>();

    private readonly int _hurtHash = Animator.StringToHash("hurt");
    private readonly int _speedHash = Animator.StringToHash("speed");
    private readonly int _dodgeHash = Animator.StringToHash("dodge");
    private readonly int _lightAttackHash = Animator.StringToHash("light_attack");
    private readonly int _heavyAttackHash = Animator.StringToHash("heavy_attack");

    // References
    public ModifierSystem ModifierSystem { get; private set; }
    public EntityInventory Inventory { get; private set; }

    void Awake() {
        ModifierSystem = GetComponent<ModifierSystem>();
        Inventory = GetComponent<EntityInventory>();
    }

    void Update() {
        recheckEffects();
        recalculateStats();
   
        // Regen
        if(Time.time - _lastDamageTime > TimeToRegenAfterDamage) {
            Health.Add(RegenRate * Time.deltaTime);
            if(Health > MaxHealth) {
                Health.Set(MaxHealth);
            }
        }
    }

    public void DropItem(InventoryItem item) {
        dropItem(item.ItemData, item.Amount);
        Inventory.RemoveInventoryItem(item);
    }

    // Spawns item at torso level and throws item on the ground
    private void dropItem(ItemData itemData, int amount) {
        ItemEntity.SpawnThrownRelative(itemData, amount, transform.position + new Vector3(0, 1.2f, 0), transform.rotation, Vector3.forward * 2);
    }

    protected void Attack(Damage damage, LivingEntity target) {
        target.TakeDamage(damage, this);
    }

    public void TakeDamage(Damage damage, LivingEntity source = null)
    {
        if (gameObject.GetComponent<PlayerController>() != null) {
            if (gameObject.GetComponent<PlayerController>().DamageDisabled) return;
        }
        if (gameObject.CompareTag("Boar")) {
            gameObject.GetComponent<Animator>()?.SetTrigger(_hurtHash);
        }
        if (gameObject.GetComponent<Rigidbody>() != null) {
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.forward * 10, ForceMode.Impulse);
        }

        gameObject.GetComponent<HitFlashAnimator>()?.Flash();
        _lastDamageTime = Time.time;

        // Check if entity is dead
        if(Health == 0) {
            return;
        }

        float desiredDamageAmount = damage.Value;
        // TODO: Calculate damage based on damage type, current entity modifiers, spells and what not

        float actualDamageAmount = desiredDamageAmount;
        if(actualDamageAmount > Health) {
            actualDamageAmount = Health;
        }

        Health.Subtract(actualDamageAmount);

        if (gameObject.GetComponent<BossHealthBar>() != null) {
            gameObject.GetComponent<BossHealthBar>().SetHealth(Health / MaxHealth);
        }

        OnDamageTaken.Invoke(new DamageTakenEventData {
            Damage = damage,
            DesiredDamageAmount = desiredDamageAmount,
            ActualDamageAmount = actualDamageAmount
        });

        if (Health == 0) {
            if(source != null) {
                source.Exp += Exp * DroppedExpMultiplier;
            }

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
                    if(humanoidInventory.Helmet != null) {
                        dropItem(humanoidInventory.Helmet, 1);
                        humanoidInventory.Helmet = null;
                    }

                    if(humanoidInventory.Chestplate != null) {
                        dropItem(humanoidInventory.Chestplate, 1);
                        humanoidInventory.Chestplate = null;
                    }

                    if(humanoidInventory.Leggings != null) {
                        dropItem(humanoidInventory.Leggings, 1);
                        humanoidInventory.Leggings = null;
                    }

                    if(humanoidInventory.Boots != null) {
                        dropItem(humanoidInventory.Boots, 1);
                        humanoidInventory.Boots = null;
                    }

                    if(humanoidInventory.Amulet != null) {
                        dropItem(humanoidInventory.Amulet, 1);
                        humanoidInventory.Amulet = null;
                    }

                    if(humanoidInventory.Ring != null) {
                        dropItem(humanoidInventory.Ring, 1);
                        humanoidInventory.Ring = null;
                    }

                    if(humanoidInventory.Weapon != null) {
                        dropItem(humanoidInventory.Weapon, 1);
                        humanoidInventory.Weapon = null;
                    }
                }
            }

            OnDeath.Invoke();

            if (gameObject.GetComponent<PlayerController>() != null) return;
            Destroy(gameObject);
        }
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

    private void recalculateStats() {
        Health.Recalculate(ModifierSystem);
        MaxHealth.Recalculate(ModifierSystem);
        RegenRate.Recalculate(ModifierSystem);
        Armor.Recalculate(ModifierSystem);
        ElementalArmor.Recalculate(ModifierSystem);
        MovementSpeed.Recalculate(ModifierSystem);
    }

    #endregion
}