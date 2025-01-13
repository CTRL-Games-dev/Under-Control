using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ModifierSystem))]
[RequireComponent(typeof(InventorySystem))]
public class LivingEntity : MonoBehaviour
{
    private struct _effectData {
        public Effect Effect;
        public float Expiration;
    }

    public string DisplayName;

    // Health
    public DynamicStat Health = new DynamicStat(StatType.HEALTH, 100);
    public Stat MaxHealth = new Stat(StatType.MAX_HEALTH, 100);
    public Stat RegenRate = new Stat(StatType.REGEN_RATE, 1);
    public float TimeToRegenAfterDamage = 2;

    // State
    private float _lastDamageTime = 0;
    private List<_effectData> _activeEffects = new List<_effectData>();

    // References
    public ModifierSystem ModifierSystem { get; private set; }
    public InventorySystem InventorySystem { get; private set; }

    // Events
    public UnityEvent OnDeath;
    public UnityEvent<DamageTakenEventData> OnDamageTaken;

    void Start()
    {
        ModifierSystem = GetComponent<ModifierSystem>();
        InventorySystem = GetComponent<InventorySystem>();
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
        _activeEffects.Add(new _effectData {
            Effect = effect,
            Expiration = Time.time + effect.Duration
        });

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
        for(int i = 0; i < _activeEffects.Count; i++) {
            if(_activeEffects[i].Expiration > Time.time) {
                continue;
            }

            _activeEffects.RemoveAt(i);
        }
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
    }

    #endregion
}