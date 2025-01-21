using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

[Serializable]
public class Stat {
    [HideInInspector, NonSerialized]
    public readonly StatType StatType;

    [SerializeField]
    private float _raw;
    public float Raw { get => _raw; protected set => _raw = value; }

    [SerializeField]
    private float _adjusted;
    public float Adjusted { get => _adjusted; protected set => _adjusted = value; }

    public Stat(StatType statType, float initValue) {
        StatType = statType;
        Raw = initValue;
        Adjusted = initValue;
    }

    public virtual float Recalculate(ModifierSystem modifierSystem) {
        Adjusted = modifierSystem.CalculateForStatType(StatType, Raw);
        return Adjusted;
    }

    public static implicit operator float(Stat stat) {
        return stat.Adjusted;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Stat))]
public class StatPropertyDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        var adjustedPropertyField = new PropertyField(property.FindPropertyRelative("_adjusted"), property.displayName);
        adjustedPropertyField.SetEnabled(false);

        return adjustedPropertyField;
    }
}
#endif