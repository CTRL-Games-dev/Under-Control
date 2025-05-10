using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Wave_Info", menuName = "Spawner/WaveInfo")]
public class WaveInfo : ScriptableObject {
    public GameObject[] EnemyPrefabs;
    public float MinInfluence = 0, MaxInfluence = 100;
}