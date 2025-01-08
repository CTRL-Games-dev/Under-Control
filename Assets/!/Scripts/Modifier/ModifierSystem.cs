using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LivingEntity))]
public class ModifierSystem : MonoBehaviour
{
    [Serializable]
    private struct ModifierData {
        public Modifier modifier;
        public float expiration;
    }


    [SerializeField] private List<ModifierData> activeModifiers = new List<ModifierData>();

    void Update() {
        for(int i = 0; i < activeModifiers.Count; i++) {
            if(activeModifiers[i].expiration > Time.time) {
                continue;
            }

            activeModifiers.RemoveAt(i);
        }
    }

    public void ApplyIndefiniteModifier(Modifier modifier) {
        activeModifiers.Add(new ModifierData {
            modifier = modifier,
            expiration = Mathf.Infinity
        });
    }

    public void ApplyTemporaryModifier(Modifier modifier, float duration) {
        activeModifiers.Add(new ModifierData {
            modifier = modifier,
            expiration = Time.time + duration
        });
    }

    public void RemoveModifier(Modifier modifier) {
        for(int i = 0; i < activeModifiers.Count; i++) {
            if(activeModifiers[i].modifier.Equals(modifier)) {
                activeModifiers.RemoveAt(i);
                return;
            }
        }
    }

    public float CalculateForStatType(StatType statType, float value) {
        for(int i = 0; i < activeModifiers.Count; i++) {
            if(activeModifiers[i].modifier.statType == statType) {
                value = activeModifiers[i].modifier.Calculate(value);
            }
        }

        return value;
    }

    public List<Modifier> GetActiveModifiers() {
        List<Modifier> modifiers = new List<Modifier>();
        for(int i = 0; i < activeModifiers.Count; i++) {
            modifiers.Add(activeModifiers[i].modifier);
        }
        return modifiers;
    }
}