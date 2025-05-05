using UnityEngine;

public static class InputUtility {
    public static bool IsMousePositionAvailable() {
        return float.IsFinite(Input.mousePosition.x) && float.IsFinite(Input.mousePosition.y);
    }
}