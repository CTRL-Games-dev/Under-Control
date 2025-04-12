using UnityEngine;

[CreateAssetMenu(fileName = "RunCard", menuName = "Cards/RunCard")]
public class Card : ScriptableObject
{
    public Sprite Icon;
    public string ModifierName;
    public string ModifierDescription;
    public float ModifierValue;
    public Modifier Modifier;
}
