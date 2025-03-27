using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.AI.Navigation;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(BetterGenerator))]
public class AdventureManager : MonoBehaviour, ILevelManager
{
    private GameManager _gameManager;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private GameObject _player;
    [SerializeField] private NavMeshSurface _navMeshSurface;
    private void Start()
    {
        _gameManager = GameManager.Instance;
        
        var generator = GetComponent<BetterGenerator>();
        generator.GenerateMap(GameManager.Instance.GetCurrentDimension());
        
        ForestPortal portal = generator.Getlocation<ForestPortal>();

        Vector2 spawn = portal.GetAbsoluteCenter(generator.wd.Offset, generator.wd.Scale);
        
        Debug.Log($"Spawn: {spawn}");
        Player.Instance.SetPlayerPosition(new Vector3(spawn.x, 1, spawn.y + 3));

        _cameraManager.SwitchCamera(Player.Instance.TopDownCamera);

        _navMeshSurface.BuildNavMesh();
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