using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public bool IsPlayer;
    public bool IsInputField;
    public bool IsOffer;
    public string Text;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Basic")]
public class Dialogue : ScriptableObject
{
    public List<DialogueEntry> DialogueEntries = new List<DialogueEntry>();

}
