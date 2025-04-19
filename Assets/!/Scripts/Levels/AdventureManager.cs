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
        Player.Instance.gameObject.SetActive(false);

        var generator = GetComponent<WorldGenerator>();
        generator.GenerateMap(GameManager.Instance.CurrentDimension);
        
        ForestPortalLocation portal = generator.Getlocation<ForestPortalLocation>();

        Vector2 spawn = portal.LocationCenterInWorld;

        Player.Instance.MaxCameraDistance = 30f;
        Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
        Player.UICanvas.ChangeUITopState(UITopState.NotVisible);
        Player.Instance.SetPlayerPosition(new Vector3(spawn.x, 1, spawn.y));

        Player.Instance.gameObject.SetActive(true);
        
        CameraManager.Instance.SwitchCamera(Player.Instance.TopDownCamera);

        _navMeshSurface.BuildNavMesh();
    }
}