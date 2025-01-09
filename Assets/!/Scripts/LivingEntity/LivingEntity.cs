using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ModifierSystem))]
[RequireComponent(typeof(InventorySystem))]
public class LivingEntity : MonoBehaviour
{
    private struct EffectData {
        public Effect effect;
        public float expiration;
    }

    public string displayName;

    // Health
    public DynamicStat health = new DynamicStat(StatType.HEALTH, 100);
    public Stat maxHealth = new Stat(StatType.MAX_HEALTH, 100);
    public Stat regenRate = new Stat(StatType.REGEN_RATE, 1);
    public float timeToRegenAfterDamage = 2;

    // State
    private float lastDamageTime = 0;
    private List<EffectData> activeEffects = new List<EffectData>();

    // References
    public ModifierSystem modifierSystem { get; private set; }
    public InventorySystem inventorySystem { get; private set; }

    // Events
    public UnityEvent OnDeath;
    public UnityEvent<DamageTakenEventData> OnDamageTaken;

    void Start()
    {
        modifierSystem = GetComponent<ModifierSystem>();
        inventorySystem = GetComponent<InventorySystem>();
    }

    void Update() {
        recheckEffects();
        recalculateStats();
   
        // Regen
        if(Time.time - lastDamageTime > timeToRegenAfterDamage) {
            health.Add(regenRate * Time.deltaTime);
            if(health > maxHealth) {
                health.Set(maxHealth);
            }
        }
    }

    public void TakeDamage(Damage damage)
    {
        lastDamageTime = Time.time;

        // Check if entity is dead
        if(health == 0) {
            return;
        }

        float desiredDamageAmount = damage.value;
        // TODO: Calculate damage based on damage type, current entity modifiers, spells and what not

        float actualDamageAmount = desiredDamageAmount;
        if(actualDamageAmount > health) {
            actualDamageAmount = health;
        }

        health.Subtract(actualDamageAmount);

        OnDamageTaken.Invoke(new DamageTakenEventData {
            damage = damage,
            desiredDamageAmount = desiredDamageAmount,
            actualDamageAmount = actualDamageAmount
        });

        if (health == 0)
        {
            OnDeath.Invoke();
        }
    }

    #region Effects

    public void ApplyEffect(Effect effect) {
        activeEffects.Add(new EffectData {
            effect = effect,
            expiration = Time.time + effect.duration
        });

        for(int i = 0; i < effect.modifiers.Length; i++) {
            var modifier = effect.modifiers[i];
            modifierSystem.ApplyTemporaryModifier(modifier, effect.duration);
        }
    }

    public void RemoveEffect(Effect effect) {
        for(int i = 0; i < activeEffects.Count; i++) {
            if(!activeEffects[i].effect.Equals(effect)) {
                continue;
            }

            for(int j = 0; j < effect.modifiers.Length; j++) {
                var modifier = effect.modifiers[j];
                modifierSystem.RemoveModifier(modifier);
            }

            activeEffects.RemoveAt(i);
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
        for(int i = 0; i < activeEffects.Count; i++) {
            if(activeEffects[i].expiration > Time.time) {
                continue;
            }

            activeEffects.RemoveAt(i);
        }
    }

    #endregion

    #region Modifiers

    public void ApplyIndefiniteModifier(Modifier modifier) {
        modifierSystem.ApplyIndefiniteModifier(modifier);
    }

    public void ApplyTemporaryModifier(Modifier modifier, float duration) {
        modifierSystem.ApplyTemporaryModifier(modifier, duration);
    }

    public void RemoveModifier(Modifier modifier) {
        modifierSystem.RemoveModifier(modifier);
    }

    private void recalculateStats() {
        health.Recalculate(modifierSystem);
        maxHealth.Recalculate(modifierSystem);
        regenRate.Recalculate(modifierSystem);
    }

    #endregion
}