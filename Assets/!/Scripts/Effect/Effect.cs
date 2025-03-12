using UnityEngine;

[CreateAssetMenu(fileName = "SO_Eff_Effect", menuName = "Effect")]
public class Effect : ScriptableObject {
    public string Name;
    public Sprite Icon;
    public float Duration;
    public Modifier[] Modifiers;

    public override string ToString() {
        if (Modifiers == null || Modifiers.Length == 0) {
            return $"{Name} ({Duration}s) [No modifiers]";
        }
        
        return $"{Name} ({Duration}s) [{string.Join(", ", Modifiers)}]";
    }
}