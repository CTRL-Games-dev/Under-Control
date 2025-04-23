using System;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class InventoryItem {
    // init-setter ensures InventoryItem<T> window remains valid
    [SerializeField, FormerlySerializedAs("ItemData")]
    private ItemData _itemData;
    public ItemData ItemData { get => _itemData; init => _itemData = value; }

    public int Amount;
    public Vector2Int Position;

    [Range(0, 2)]
    public float PowerScale = 1;
    public bool Rotated;

    public Vector2Int Size { get => ItemData.Size; }
    public int ScaledValue { get => (int)(ItemData.Value * PowerScale); }

    public ItemUI ItemUI { get; set; }
    public RectTransform RectTransform { get; set; }

    public InventoryItem<T> As<T>() where T : ItemData {
        if(TryAs(out InventoryItem<T> inventoryItem)) {
            return inventoryItem;
        }

        throw new InvalidCastException();
    }

    public bool TryAs<T>(out InventoryItem<T> inventoryItem) where T : ItemData {
        if(ItemData is T itemData) {
            inventoryItem = new InventoryItem<T>(this, itemData);
            return true;
        }

        inventoryItem = default;
        return false;
    }
}

// It's a window into the InventoryItem!!!
[Serializable]
public class InventoryItem<T> where T : ItemData {
    [SerializeField]
    private InventoryItem _inventoryItem;

    public T ItemData { get; private init; }
    public int Amount { get => _inventoryItem.Amount; set => _inventoryItem.Amount = value; }
    public Vector2Int Position { get => _inventoryItem.Position; set => _inventoryItem.Position = value; }

    [Range(0, 2)]
    public float PowerScale { get => _inventoryItem.PowerScale; set => _inventoryItem.PowerScale = value; }
    public bool Rotated { get => _inventoryItem.Rotated; set => _inventoryItem.Rotated = value; }

    public Vector2Int Size { get => ItemData.Size; }
    public int ScaledValue { get => _inventoryItem.ScaledValue; }

    public ItemUI ItemUI { get => _inventoryItem.ItemUI; set => _inventoryItem.ItemUI = value; }
    public RectTransform RectTransform { get => _inventoryItem.RectTransform; set => _inventoryItem.RectTransform = value; }

    public InventoryItem(InventoryItem source) {
        _inventoryItem = source;
        ItemData = source.ItemData as T;
    }

    public InventoryItem(InventoryItem source, T itemData) {
        _inventoryItem = source;
        ItemData = itemData;
    }

    public static implicit operator InventoryItem(InventoryItem<T> inventoryItem) {
        return inventoryItem._inventoryItem;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(InventoryItem<>))]
public class InventoryItemDrawer : PropertyDrawer
{
    private string _genericName;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if(_genericName == null) {
            _genericName = property.boxedValue.GetType().GenericTypeArguments[0].Name;
        }

        label.text = $"{label.text} (Restricted to {_genericName})";

        SerializedProperty innerProperty = property.FindPropertyRelative("_inventoryItem");

        EditorGUI.PropertyField(position, innerProperty, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        SerializedProperty innerProperty = property.FindPropertyRelative("_inventoryItem");

        return EditorGUI.GetPropertyHeight(innerProperty);
    }
}
#endif