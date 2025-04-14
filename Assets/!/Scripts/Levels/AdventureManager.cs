using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(WorldGenerator))]
public class AdventureManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private NavMeshSurface _navMeshSurface;
    private void Start()
    {
        var generator = GetComponent<WorldGenerator>();
        generator.GenerateMap(GameManager.Instance.CurrentDimension);
        
        ForestPortalLocation portal = generator.Getlocation<ForestPortalLocation>();

        Vector2 spawn = portal.LocationCenterInWorld;

        Player.Instance.MaxCameraDistance = 30f;
        Player.Instance.SetPlayerPosition(new Vector3(spawn.x, 1, spawn.y));
        CameraManager.Instance.SwitchCamera(Player.Instance.TopDownCamera);

        _navMeshSurface.BuildNavMesh();
    }
}