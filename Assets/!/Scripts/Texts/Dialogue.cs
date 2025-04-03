using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public bool IsPlayer;
    public string Name;
    public string Text;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Basic")]
public class Dialogue : ScriptableObject
{

    public List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();
}
