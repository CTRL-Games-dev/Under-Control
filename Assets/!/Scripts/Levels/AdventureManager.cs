using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class AdventureManager : MonoBehaviour, ILevelManager
{
    // This is just a place holder to see if portals work
    // In main menu player will not be spawned
    public Vector3 PlayerStartingPos = new(3,3,3);
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _portal;
    // [SerializeField] private CameraManager _cameraManager;
    private GameManager _gm;

    [Range(10, 300)]
    public int width = 10, height = 10;
    [Range(1, 20)]
    public int iterations = 0;

    // Terrain
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;
    private MeshFilter _mesh;

    // Enemies
    [SerializeField] private GameObject _enemyPrefab;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = GetComponent<MeshFilter>();

        // Get game manager
        _gm = GameManager.instance;
    }
    private void Start()
    {
        var s = new BetterGenerator();
        s.GenerateMap(BetterGenerator.LevelType.Forest);
    }

    private void Update()
    {
        
    }

    // public void SpawnPlayer()
    // {
        // GameObject player = Instantiate(_player, _map.SpawnLocation, Quaternion.identity);
        // GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        // player.GetComponent<PlayerController>().CameraObject = camera;
        // camera.GetComponent<CinemachineCamera>().Follow = player.transform;
    // }
    // private void SpawnEnemies(WorldMap map)
    // {
    //     int groupCount = _gm.GetInfluence() > 0.5 ? 2 : 3;

    //     var floors = map.GetMapAsList().Where(x => x.Type == TileType.FLOOR);
    //     var floorCount = floors.Count();

    //     Debug.Log(floorCount);

    //     for(int i = 0; i < groupCount; i++)
    //     {
    //         int floorIndex = UnityEngine.Random.Range(0, floorCount);
    //         var tile = floors.ElementAt(floorIndex);


    //         var center = tile.GetCenter(_map.TileWidth);
    //         var enemyCount = UnityEngine.Random.Range(3, 6);
    //         for(int e = 0; e < enemyCount; e++)
    //         {
    //             float left = e%3*1.5f - 1.5f;
    //             float bottom = e/3*1.5f - 1.5f;
    //             Instantiate(_enemyPrefab, new Vector3(center.x + left, 1, center.y + bottom), Quaternion.identity);
    //         }
    //     }
    // }
}