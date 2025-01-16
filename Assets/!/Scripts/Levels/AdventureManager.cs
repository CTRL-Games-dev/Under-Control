using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class AdventureManager : MonoBehaviour, ILevelManager
{
    public Vector3 playerStartingPos = new(3,3,3);
    public readonly float DefaultTileWidth = 4.0f;
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

    // Enemies
    [SerializeField] private GameObject _enemyPrefab;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = GetComponent<MeshFilter>();
    }
    private void Start()
    {
        // Get game manager
        _gm = GameManager.gm;

        // Generate map and render it
        _map = MapGenerator.GetMap(width, height, iterations, DefaultTileWidth);
        _map.Grass = _grassMaterial;
        _map.Walls = _wallMaterial;
        _map.Generate(_meshRenderer, _mesh, _meshCollider);

        Debug.Log(_map.spawnLocation);
        SpawnPlayer();
        // Spawn enemies
        SpawnEnemies(_map);
    }

    private void Update()
    {
        
    }

    public void SpawnPlayer()
    {
        GameObject.Instantiate(_player, _map.spawnLocation, Quaternion.identity);
        GameObject.Instantiate(_portal, _map.spawnLocation + new Vector3(-3.5f, 0, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
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
            Debug.Log(tile.X + " " + tile.Y);
            Debug.Log(center);

            Instantiate(_enemyPrefab, new Vector3(center.x, 1, center.y), Quaternion.identity);
        }
    }
}