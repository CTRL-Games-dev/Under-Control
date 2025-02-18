using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LivingEntity))]
public class ModifierSystem : MonoBehaviour
{
    [Serializable]
    private struct _modifierData {
        public Modifier Modifier;
        public float Expiration;
    }


    [SerializeField] private List<_modifierData> _activeModifiers = new List<_modifierData>();

    void Update() {
        for(int i = 0; i < _activeModifiers.Count; i++) {
            if(_activeModifiers[i].Expiration > Time.time) {
                continue;
            }

            _activeModifiers.RemoveAt(i);
        }
    }

    public void ApplyIndefiniteModifier(Modifier modifier) {
        _activeModifiers.Add(new _modifierData {
            Modifier = modifier,
            Expiration = Mathf.Infinity
        });
    }

    public void ApplyTemporaryModifier(Modifier modifier, float duration) {
        _activeModifiers.Add(new _modifierData {
            Modifier = modifier,
            Expiration = Time.time + duration
        });
    }

    public void RemoveModifier(Modifier modifier) {
        for(int i = 0; i < _activeModifiers.Count; i++) {
            if(_activeModifiers[i].Modifier.Equals(modifier)) {
                _activeModifiers.RemoveAt(i);
                return;
            }
        }
    }

    public float CalculateForStatType(StatType statType, float value) {
        for(int i = 0; i < _activeModifiers.Count; i++) {
            if(_activeModifiers[i].Modifier.StatType == statType) {
                value = _activeModifiers[i].Modifier.Calculate(value);
            }
        }

        return value;
    }

    public List<Modifier> GetActiveModifiers() {
        List<Modifier> modifiers = new List<Modifier>();
        for(int i = 0; i < _activeModifiers.Count; i++) {
            modifiers.Add(_activeModifiers[i].Modifier);
        }
        return modifiers;
    }
}