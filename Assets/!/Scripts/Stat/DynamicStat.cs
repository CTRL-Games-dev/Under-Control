using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;    
#endif

// Dynamic stat is a stat that can be changed by external means
// It's expected to be unstable with multiplicative and percentage modifiers
// I might disable multiplicative and percentage modifiers for dynamic stats
// in the future
// - @londek
[Serializable]
public class DynamicStat : Stat {
    [SerializeField]
    private float _raw;
    public float Raw { get => _raw; protected set => _raw = value; }

    public DynamicStat(StatType statType, float initValue) : base(statType, initValue) {
        Raw = initValue;
    }

    public void Add(float value) {
        Raw += value;
        Adjusted += value;
    }

    public void Subtract(float value) {
        Raw -= value;
        Adjusted -= value;
    }

    public void Multiply(float value) {
        Raw *= value;
        Adjusted *= value;
    }

    public void Divide(float value) {
        Raw /= value;
        Adjusted /= value;
    }

    // Beware! Potential magic behavior.
    // Difference between raw and value is added to adjusted value
    // Just saying, this is not "force" set
    public void Set(float value) {
        var difference = value - Raw;
        Raw = value;
        Adjusted += difference;
    }

    public override float Recalculate(ModifierSystem modifierSystem) {
        Adjusted = modifierSystem.CalculateForStatType(StatType, Raw);
        return Adjusted;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DynamicStat))]
public class DynamicStatPropertyDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        var propertiesContainer = new Foldout { text = property.displayName, value = true };

        var statTypePropertyField = new PropertyField(property.FindPropertyRelative("statType"));
        propertiesContainer.Add(statTypePropertyField);

        var rawPropertyField = new PropertyField(property.FindPropertyRelative("_raw"));
        propertiesContainer.Add(rawPropertyField);

        var adjustedPropertyField = new PropertyField(property.FindPropertyRelative("_adjusted"));
        adjustedPropertyField.SetEnabled(false);
        propertiesContainer.Add(adjustedPropertyField);

        return propertiesContainer;
    }
}
#endif