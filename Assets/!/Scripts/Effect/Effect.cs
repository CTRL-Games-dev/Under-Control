using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect")]
public class Effect : ScriptableObject {
    public string displayName;
    public Sprite icon;
    public float duration;
    public Modifier[] modifiers;

    public override string ToString() {
        return $"{displayName} ({duration}s) [{string.Join(", ", modifiers)}]";
    }
}