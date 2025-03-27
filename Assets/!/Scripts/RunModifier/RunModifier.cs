using UnityEngine;

[CreateAssetMenu(fileName = "RunModifier", menuName = "RunModifiers/RunModifier")]
public class RunModifier : ScriptableObject
{
    public Sprite Icon;
    public string ModifierName;
    public string ModifierDescription;
    public float ModifierValue;
    public RunStat Modifier;
}
