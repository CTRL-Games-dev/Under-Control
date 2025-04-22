using UnityEngine;

public abstract class Card : ScriptableObject
{
    public Sprite Icon;
    public string DisplayName;
    public string ShortDesc;
    public string LongDesc;
    public ElementalType ElementalType;
}
