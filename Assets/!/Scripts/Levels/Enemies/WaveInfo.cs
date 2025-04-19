using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Wave_Info", menuName = "Spawner/WaveInfo")]
public class WaveInfo : ScriptableObject {
    public List<EnemySpawnInfo> EnemyInfo;
    public int MinEnemies, MaxEnemies;
}