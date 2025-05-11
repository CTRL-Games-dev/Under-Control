using System.Collections.Generic;
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
    private WorldGenerator _generator;
    public static AdventureManager Instance;

    void Awake() {
        Instance = this;
    }
    void Start() {
        AudioManager.instance.setMusicArea(MusicArea.EXPLORING);
        GameManager.Instance.LevelDepth++;

        Player.Instance.gameObject.SetActive(false);

        _generator = GetComponent<WorldGenerator>();
        _generator.GenerateMap(GameManager.Instance.CurrentDimension);
        
        ForestSpawnLocation spawnLocation = _generator.GetLocation<ForestSpawnLocation>();

        if(GameManager.Instance.LevelDepth == 1) {
            Vector3 weaponSpawnPosition = spawnLocation.transform.Find("ItemSpawn").position;
            WeaponItemData randomWeapon = PossibleStartingItems[UnityEngine.Random.Range(0, PossibleStartingItems.Count())]; 
            ItemEntity.Spawn(randomWeapon, 1, weaponSpawnPosition, ItemRandomizer.GetStartPowerScale(), Quaternion.Euler(new(0, 120, 30)));
        }

        Vector2 spawn = spawnLocation.LocationCenterInWorld;

        Player.Instance.MaxCameraDistance = 30f;
        Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
        Player.UICanvas.ChangeUITopState(UITopState.NotVisible);
        Player.Instance.SetPlayerPosition(new Vector3(spawn.x, 1, spawn.y));

        Player.Instance.gameObject.SetActive(true);
        
        CameraManager.SwitchCamera(Player.Instance.TopDownCamera);

        _navMeshSurface.BuildNavMesh();

        Player.Instance.ResetToDefault();
        setPortals();

        Invoke(nameof(sceneReady), 0.2f);
    }

    private void setPortals() {
        List<ForestPortalLocation> portals = _generator.GetAllLocations<ForestPortalLocation>();
        
        int bossesDefeated = GameManager.Instance.BossesDefeated;
        float influence = GameManager.Instance.TotalInfluence;

        Dimension dimension;
        if(influence >= 100 && bossesDefeated == 2) {
            dimension = Dimension.VEKTHAR_BOSS;
        } else if(influence >= 66 && bossesDefeated == 1) {
            dimension = Dimension.ENT_BOSS;
        } else if(influence >= 33 && bossesDefeated == 0) {
            dimension = Dimension.SLIME_BOSS;
        } else {
            dimension = Dimension.CARD_CHOOSE;
        }

        foreach(var p in portals) {
            p.Portal.SetDimension(dimension);
        }
    }

    private void sceneReady() {
        EventBus.SceneReadyEvent?.Invoke();
        Player.LivingEntity.Mana = Player.LivingEntity.MaxMana;
    }

    public static void ReturnPlayerToStart() {
        ForestPortalLocation portal = Instance._generator.GetLocation<ForestPortalLocation>();

        var pos = portal.LocationCenterInWorld;

        Player.Instance.SetPlayerPosition(new Vector3(pos.x, 1, pos.y));
        Player.LivingEntity.TakeDamage(new Damage {
            Type = DamageType.TRUE_DAMAGE,
            Value = 33,
        });
    }
}