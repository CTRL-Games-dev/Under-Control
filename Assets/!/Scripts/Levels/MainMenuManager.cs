using UnityEngine;

public class MainMenuManager : MonoBehaviour, ILevelManager
{
    // This is just a place holder to see if portals work
    // In main menu player will not be spawned
    public Vector3 playerStartingPos = new(3,3,3);
    [SerializeField] private GameObject _player;
    [SerializeField] private CameraManager _cameraManager;
    private void Start()
    {
        Instantiate(_player, playerStartingPos, Quaternion.identity);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _cameraManager.Target = player.transform;
    }

    private void Update()
    {
        
    }
}