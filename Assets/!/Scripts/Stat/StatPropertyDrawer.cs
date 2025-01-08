using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Stat))]
public class StatPropertyDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        var container = new VisualElement();

        container.Add(new Label(property.displayName));

        var propertiesContainer = new Foldout { text = property.displayName, value = true };

        var statTypePropertyField = new PropertyField(property.FindPropertyRelative("statType"));
        propertiesContainer.Add(statTypePropertyField);

        var rawPropertyField = new PropertyField(property.FindPropertyRelative("_raw"));
        propertiesContainer.Add(rawPropertyField);

        var adjustedPropertyField = new PropertyField(property.FindPropertyRelative("_adjusted"));
        adjustedPropertyField.SetEnabled(false);
        propertiesContainer.Add(adjustedPropertyField);

        container.Add(propertiesContainer);

        return propertiesContainer;
    }
}