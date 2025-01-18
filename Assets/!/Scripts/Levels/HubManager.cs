using UnityEngine;

public class HubManager : MonoBehaviour, ILevelManager
{    public Vector3 PlayerStartingPos = new(3,3,3);
    [SerializeField] private GameObject _player;
    [SerializeField] private CameraManager _cameraManager;
    private void Start()
    {
        Instantiate(_player, PlayerStartingPos, Quaternion.identity);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _cameraManager.Target = player.transform;
    }

    private void Update()
    {
        
    }
}