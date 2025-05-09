using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(WorldGenerator))]
public class AdventureManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private NavMeshSurface _navMeshSurface;
    public WeaponItemData[] PossibleStartingItems;
    
    private void Start() {
        GameManager.Instance.LevelDepth++;

        Player.Instance.gameObject.SetActive(false);

        var generator = GetComponent<WorldGenerator>();
        generator.GenerateMap(GameManager.Instance.CurrentDimension);
        
        ForestPortalLocation portal = generator.Getlocation<ForestPortalLocation>();

        if(GameManager.Instance.LevelDepth == 1) {
            Vector3 weaponSpawnPosition = portal.transform.Find("ItemSpawn").position;
            WeaponItemData randomWeapon = PossibleStartingItems[UnityEngine.Random.Range(0, PossibleStartingItems.Count())]; 
            ItemEntity.Spawn(randomWeapon, 1, weaponSpawnPosition, ItemRandomizer.GetPowerScale(), Quaternion.Euler(new(0, 120, 30)));
        }

        Vector2 spawn = portal.LocationCenterInWorld;

        Player.Instance.MaxCameraDistance = 30f;
        Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
        Player.UICanvas.ChangeUITopState(UITopState.NotVisible);
        Player.Instance.SetPlayerPosition(new Vector3(spawn.x, 1, spawn.y));

        Player.Instance.gameObject.SetActive(true);
        
        CameraManager.SwitchCamera(Player.Instance.TopDownCamera);

        _navMeshSurface.BuildNavMesh();

        Player.Instance.ResetToDefault();
        Player.LivingEntity.Mana = Player.LivingEntity.MaxMana;

        Invoke(nameof(sceneReady), 0.2f);
    }

    private void sceneReady() {
        EventBus.SceneReadyEvent?.Invoke();
    }
}