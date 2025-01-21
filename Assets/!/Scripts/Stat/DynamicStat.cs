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
// ile ja nad tym pracowalem wtf
[CustomPropertyDrawer(typeof(DynamicStat))]
public class DynamicStatPropertyDrawer : PropertyDrawer {
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