using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ModifierSystem : MonoBehaviour
{
    [Serializable]
    private struct ModifierData {
        public Modifier Modifier;
        public float Expiration;
    }

    [SerializeField] private List<ModifierData> _activeModifiers = new List<ModifierData>();
    [SerializeField, HideInInspector] private List<Stat> _stats = new List<Stat>();

    private bool _statsModified = false;

    void Update() {
        for(int i = 0; i < _activeModifiers.Count; i++) {
            if(_activeModifiers[i].Expiration > Time.time) {
                continue;
            }

            _activeModifiers.RemoveAt(i);

            _statsModified = true;
        }

        if(_statsModified) {
            for(int i = 0; i < _stats.Count; i++) {
                _stats[i].Recalculate(this);
            }

            _statsModified = false;
        }
    }

    public void ApplyIndefiniteModifier(Modifier modifier) {
        _activeModifiers.Add(new ModifierData {
            Modifier = modifier,
            Expiration = Mathf.Infinity
        });

        _statsModified = true;
    }

    public void ApplyTemporaryModifier(Modifier modifier, float duration) {
        _activeModifiers.Add(new ModifierData {
            Modifier = modifier,
            Expiration = Time.time + duration
        });

        _statsModified = true;
    }

    public void RemoveModifier(Modifier modifier) {
        for(int i = 0; i < _activeModifiers.Count; i++) {
            if(_activeModifiers[i].Modifier.Equals(modifier)) {
                _activeModifiers.RemoveAt(i);
                _statsModified = true;
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

    public void Reset() {
        _activeModifiers.Clear();

        for(int i = 0; i < _stats.Count; i++) {
            _stats[i].Reset();
        }

        _statsModified = false;
    }

    public void Recalculate() {
        for(int i = 0; i < _stats.Count; i++) {
            _stats[i].Recalculate(this);
        }
    }

    public void RegisterStat(ref Stat stat) {
        _stats.Add(stat);
        stat.Recalculate(this);
    }

    public void RegisterStat(ref DynamicStat stat) {
        _stats.Add(stat);
        stat.Recalculate(this);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ModifierSystem))]
public class ModifierSystemEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        ModifierSystem modifierSystem = (ModifierSystem) target;

        if(GUILayout.Button("Recalculate")) {
            modifierSystem.Recalculate();
        }

        if(GUILayout.Button("Reset")) {
            modifierSystem.Reset();
        }
    }
}
#endif