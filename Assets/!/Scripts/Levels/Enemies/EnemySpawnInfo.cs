using UnityEngine;

[CreateAssetMenu(fileName = "SO_Enemy_Spawn_Info", menuName = "Spawner/EnemySpawnInfo")]
public class EnemySpawnInfo : ScriptableObject {
    public GameObject EnemyPrefab;
    public float minInfluence, maxInfluence;
}