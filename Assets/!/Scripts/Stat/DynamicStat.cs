using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;    
#endif

// Dynamic stat is a stat that can be changed by external means
// It's expected to be unstable with multiplicative and percentage modifiers
// I might disable multiplicative and percentage modifiers for dynamic stats
// in the future
// - @londek
[Serializable]
public class DynamicStat : Stat {
    public DynamicStat(StatType statType) : base(statType) {}

    public void Add(float value) {
        Raw += value;
        Adjusted += value;

        OnValueChanged?.Invoke();
    }

    public void Subtract(float value) {
        Raw -= value;
        Adjusted -= value;

        OnValueChanged?.Invoke();
    }

    public void Multiply(float value) {
        Raw *= value;
        Adjusted *= value;

        OnValueChanged?.Invoke();
    }

    public void Divide(float value) {
        Raw /= value;
        Adjusted /= value;

        OnValueChanged?.Invoke();
    }

    // Beware! Potential magic behavior.
    // Difference between raw and value is added to adjusted value
    // Just saying, this is not "force" set
    public void Set(float value) {
        var difference = value - Raw;
        Raw = value;
        Adjusted += difference;
    }
}

#if UNITY_EDITOR
// ile ja nad tym pracowalem wtf
[CustomPropertyDrawer(typeof(DynamicStat))]
public class DynamicStatPropertyDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Same logic as for Stat, for now
        return new StatPropertyField(property);
    }
}
#endif