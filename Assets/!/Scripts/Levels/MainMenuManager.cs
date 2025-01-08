using UnityEngine;

public class MainMenuManager : MonoBehaviour, LevelManager
{
    // This is just a place holder to see if portals work
    // In main menu player will not be spawned
    public Vector3 playerStartingPos = new(3,3,3);
    [SerializeField] private GameObject player;
    private void Start()
    {
        Instantiate(player, playerStartingPos, Quaternion.identity);
    }

    private void Update()
    {
        
    }
}