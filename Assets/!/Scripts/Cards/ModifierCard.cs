using UnityEngine;

[CreateAssetMenu(fileName = "ModifierCard", menuName = "Cards/ModifierCard")]

public class ModifierCard : Card
{
    public float ModifierValue;
    public Modifier Modifier;

    public override string ToString()
    {
        return $"{DisplayName} ({ModifierValue}) [{Modifier}]";
    }
}