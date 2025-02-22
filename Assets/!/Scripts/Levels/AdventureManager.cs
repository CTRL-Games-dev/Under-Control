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
    [SerializeField] private CameraManager _cameraManager;
    private GameManager _gm;

    [Range(10, 300)]
    public int width = 10, height = 10;
    [Range(1, 20)]
    public int iterations = 0;

    // Terrain
    [SerializeField] private Material _grassMaterial;
    [SerializeField] private Material _wallMaterial;
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;
    private MeshFilter _mesh;
    private WorldMap _map;
    public readonly float DefaultTileWidth = 5.0f; 

    // Enemies
    [SerializeField] private GameObject _enemyPrefab;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = GetComponent<MeshFilter>();

        // Get game manager
        _gm = GameManager.instance;

        // Generate map and render it
        _map = MapGenerator.GetMap(width, height, iterations, DefaultTileWidth);
        _map.Grass = _grassMaterial;
        _map.Walls = _wallMaterial;
        _map.Generate(_meshRenderer, _mesh, _meshCollider);

        Debug.Log(_map.SpawnLocation);
        SpawnPlayer();
        // Spawn enemies
        SpawnEnemies(_map);
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void SpawnPlayer()
    {
        GameObject player = Instantiate(_player, _map.SpawnLocation, Quaternion.identity);
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        player.GetComponent<PlayerController>().CameraObject = camera;
        camera.GetComponent<CinemachineCamera>().Follow = player.transform;
        
        Instantiate(_portal, _map.SpawnLocation + new Vector3(-3.5f, 0, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
        // GameObject player = GameObject.FindGameObjectWithTag("Player");
        // _cameraManager.Target = player.transform;
    }
    private void SpawnEnemies(WorldMap map)
    {
        int groupCount = _gm.GetInfluence() > 0.5 ? 2 : 3;

        var floors = map.GetMapAsList().Where(x => x.Type == TileType.FLOOR);
        var floorCount = floors.Count();

        Debug.Log(floorCount);

        for(int i = 0; i < groupCount; i++)
        {
            int floorIndex = UnityEngine.Random.Range(0, floorCount);
            var tile = floors.ElementAt(floorIndex);


            var center = tile.GetCenter(_map.TileWidth);
            var enemyCount = UnityEngine.Random.Range(3, 6);
            for(int e = 0; e < enemyCount; e++)
            {
                float left = e%3*1.5f - 1.5f;
                float bottom = e/3*1.5f - 1.5f;
                Instantiate(_enemyPrefab, new Vector3(center.x + left, 1, center.y + bottom), Quaternion.identity);
            }
        }
    }
}