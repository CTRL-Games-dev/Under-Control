using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Guild", menuName = "Guild")]
public class Guild : ScriptableObject {
    public string Name;
    public List<Guild> HostileTowards;
    public bool HostileTowardsEveryone;

    public bool IsHostileTowards(Guild guild) {
        if(HostileTowardsEveryone) {
            return true;
        }

        return HostileTowards.Contains(guild);
    }
}