using Unity.Cinemachine;
using UnityEngine;

public class HubManager : MonoBehaviour, ILevelManager
{    
    public Vector3 PlayerStartingPos = new(0,0,0);
    [SerializeField] private GameObject _player;
    private void Start()
    {
        SpawnPlayer();
    }

    private void Update()
    {
        
    }

    private void SpawnPlayer()
    {
        GameObject player = Instantiate(_player, PlayerStartingPos, Quaternion.identity);
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        
        player.GetComponent<PlayerController>().CameraObject = camera;
        camera.GetComponent<CinemachineCamera>().Follow = player.transform;
    }
}