using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ModifierSystem))]
public class LivingEntity : MonoBehaviour
{
    private struct EffectData {
        public Effect Effect;
        public float Expiration;
    }

    [Header("Properties")]
    public string DisplayName;
    public float TimeToRegenAfterDamage = 2;

    [SerializeField]
    private EntityInventory _inventory = new EntityInventory();
    public EntityInventory Inventory { get => _inventory; private set => _inventory = value; }

    public int Exp = 0;
    public float Level { get => 1 + Exp / 100f; }

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

    // References
    public ModifierSystem ModifierSystem { get; private set; }

    void Start()
    {
        ModifierSystem = GetComponent<ModifierSystem>();
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

    public void TakeDamage(Damage damage)
    {
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

        OnDamageTaken.Invoke(new DamageTakenEventData {
            Damage = damage,
            DesiredDamageAmount = desiredDamageAmount,
            ActualDamageAmount = actualDamageAmount
        });

        if (Health == 0)
        {
            OnDeath.Invoke();
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