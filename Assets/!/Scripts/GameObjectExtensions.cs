using UnityEngine;

public static class GameObjectExtensions {
    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component) where T : Component {
        component = gameObject.GetComponentInParent<T>();
        return component != null;
    }
}