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
// Source: UnityEditor.UIElements.PropertyField
public class StatPropertyField : PropertyField {
    public static readonly string noLabelVariantUssClassName = ussClassName + "--no-label";

    public static readonly string labelDraggerVariantUssClassName = labelUssClassName + "--with-dragger";

    public static readonly string mixedValueLabelUssClassName = labelUssClassName + "--mixed-value";

    public static readonly string alignedFieldUssClassName = ussClassName + "__aligned";

    public static readonly string inspectorFieldUssClassName = ussClassName + "__inspector-field";

    private VisualElement m_CachedContextWidthElement;
    private VisualElement m_CachedInspectorElement;
    
    private Label labelElement;

    public StatPropertyField(SerializedProperty property) {
        var labelWidth = EditorGUIUtility.labelWidth;

        RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        RegisterCallback<GeometryChangedEvent>(OnInspectorFieldGeometryChanged);

        AddToClassList("unity-base-field");
        AddToClassList("unity-base-composite-field");
        AddToClassList("unity-base-field__aligned");
        AddToClassList("unity-base-field__inspector-field");
        AddToClassList(alignedFieldUssClassName);

        var testField = new PropertyField();

        labelElement = new Label(property.displayName);
        labelElement.AddToClassList("unity-base-field__label");
        labelElement.AddToClassList("unity-base-text-field__label");
        labelElement.AddToClassList("unity-float-field__label");
        labelElement.AddToClassList("unity-base-field__label--with-dragger");
        labelElement.AddToClassList("unity-property-field__label");

        labelElement.style.width = labelWidth;

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

        Add(labelElement);
        Add(inputContainer);
    }

    private void OnAttachToPanel(AttachToPanelEvent evt) {
        m_CachedInspectorElement = null;
        m_CachedContextWidthElement = null;
        for (VisualElement visualElement = base.parent; visualElement != null; visualElement = visualElement.parent)
        {
            if (visualElement.ClassListContains("unity-inspector-element"))
            {
                m_CachedInspectorElement = visualElement;
            }

            if (visualElement.ClassListContains("unity-inspector-main-container"))
            {
                m_CachedContextWidthElement = visualElement;
                break;
            }
        }
    }

    private void OnInspectorFieldGeometryChanged(GeometryChangedEvent e)
    {
        AlignLabel();
    }

    private void AlignLabel()
    {
        if (ClassListContains(alignedFieldUssClassName) && m_CachedInspectorElement != null)
        {

            float labelExtraPadding = 37; // m_LabelExtraPadding;
            float num = worldBound.x - m_CachedInspectorElement.worldBound.x - m_CachedInspectorElement.resolvedStyle.paddingLeft;
            labelExtraPadding += num;
            labelExtraPadding += resolvedStyle.paddingLeft;
            float a = 120 - num - resolvedStyle.paddingLeft;
            VisualElement visualElement = m_CachedContextWidthElement ?? m_CachedInspectorElement;
            labelElement.style.minWidth = Mathf.Max(a, 0f);
            float num2 = Mathf.Ceil(visualElement.resolvedStyle.width * 0.45f) - labelExtraPadding;
            if (Mathf.Abs(labelElement.resolvedStyle.width - num2) > 1E-30f)
            {
                labelElement.style.width = Mathf.Max(0f, num2);
            }
        }
    }
}

// ile ja nad tym pracowalem wtf
[CustomPropertyDrawer(typeof(Stat))]
public class StatPropertyDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return new StatPropertyField(property);
    }
}
#endif