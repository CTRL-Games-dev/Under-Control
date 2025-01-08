using UnityEngine;

public class HubManager : MonoBehaviour, LevelManager
{    public Vector3 playerStartingPos = new(3,3,3);
    [SerializeField] private GameObject player;
    private void Start()
    {
        Instantiate(player, playerStartingPos, Quaternion.identity);
    }

    private void Update()
    {
        
    }
}