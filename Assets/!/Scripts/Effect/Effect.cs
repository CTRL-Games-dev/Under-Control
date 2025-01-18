using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect")]
public class Effect : ScriptableObject {
    public string DisplayName;
    public Sprite Icon;
    public float Duration;
    public Modifier[] Modifiers;

    public override string ToString() {
        if (Modifiers == null || Modifiers.Length == 0) {
            return $"{DisplayName} ({Duration}s) [No modifiers]";
        }
        
        return $"{DisplayName} ({Duration}s) [{string.Join(", ", Modifiers)}]";
    }
}