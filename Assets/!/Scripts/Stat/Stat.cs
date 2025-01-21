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
// ile ja nad tym pracowalem wtf
[CustomPropertyDrawer(typeof(Stat))]
public class StatPropertyDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Hack to get the ACTUAL label width
        bool hierarchyMode = EditorGUIUtility.hierarchyMode;
        EditorGUIUtility.hierarchyMode = true;
        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.hierarchyMode = hierarchyMode;

        var container = new VisualElement();
        container.AddToClassList("unity-base-field");
        container.AddToClassList("unity-base-composite-field");
        container.AddToClassList("unity-base-field__aligned");
        container.AddToClassList("unity-base-field__inspector-field");

        var label = new Label(property.displayName);
        label.AddToClassList("unity-base-field__label");
        label.AddToClassList("unity-base-text-field__label");
        label.AddToClassList("unity-float-field__label");
        label.AddToClassList("unity-base-field__label--with-dragger");
        label.AddToClassList("unity-property-field__label");

        label.style.width = labelWidth;

        var inputContainer = new VisualElement();
        inputContainer.style.flexDirection = FlexDirection.Row;

        inputContainer.AddToClassList("unity-base-field__input");
        inputContainer.AddToClassList("unity-property-field__input");
        inputContainer.AddToClassList("unity-composite-field__input");
        inputContainer.AddToClassList("unity-vector2-field__input");
        
        // // Create property fields
        var rawField = new FloatField();
        rawField.style.flexGrow = 1;
        rawField.style.flexBasis = 0;
        rawField.style.marginLeft = 0;
        rawField.BindProperty(property.FindPropertyRelative("_raw"));
        
        var arrowLabel = new Label("=>");
        arrowLabel.style.width = 30;
        arrowLabel.style.flexGrow = 0;
        arrowLabel.style.paddingLeft = 0;
        arrowLabel.style.paddingRight = 0;
        arrowLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        var adjustedField = new FloatField();
        adjustedField.style.flexBasis = 0;
        adjustedField.style.flexGrow = 1;
        adjustedField.style.marginLeft = 0;
        adjustedField.BindProperty(property.FindPropertyRelative("_adjusted"));
        adjustedField.SetEnabled(false);
        
        inputContainer.Add(rawField);
        inputContainer.Add(arrowLabel);
        inputContainer.Add(adjustedField);

        container.Add(label);
        container.Add(inputContainer);
        
        return container;
    }
}
#endif