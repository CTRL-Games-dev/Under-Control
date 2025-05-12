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
    public static Vector3 SpawnPosition; 

    void Awake() {
        Instance = this;
    }
    
    void Start() {
        AudioManager.instance.setMusicArea(MusicArea.EXPLORING);
        GameManager.Instance.LevelDepth++;

        Player.Instance.FBXModel.SetActive(false);

        _generator = GetComponent<WorldGenerator>();
        _generator.GenerateMap(GameManager.Instance.CurrentDimension);
        
        ForestSpawnLocation spawnLocation = _generator.GetLocation<ForestSpawnLocation>();

        if(GameManager.Instance.LevelDepth == 1) {
            Vector3 weaponSpawnPosition = spawnLocation.transform.Find("ItemSpawn").position;
            WeaponItemData randomWeapon = PossibleStartingItems[Random.Range(0, PossibleStartingItems.Count())]; 
            ItemEntity.Spawn(randomWeapon, 1, weaponSpawnPosition, ItemRandomizer.GetStartPowerScale(), Quaternion.Euler(new(0, 120, 30)));
        }

        AdventureManager.SpawnPosition  = new Vector3(spawnLocation.LocationCenterInWorld.x, 1, spawnLocation.LocationCenterInWorld.y);

        Player.Instance.MaxCameraDistance = 30f;
        Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
        Player.UICanvas.ChangeUITopState(UITopState.NotVisible);

        Player.Instance.FBXModel.SetActive(true);
        Player.Instance.SetPlayerPosition(AdventureManager.SpawnPosition);

        Player.UICanvas.HUDCanvas.UpdateIndicatorImg();
        
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
        if(influence >= 100 && bossesDefeated == 1) {
            dimension = Dimension.VEKTHAR_BOSS;
        } else if(influence >= 50 && bossesDefeated == 0) {
            dimension = Dimension.ENT_BOSS;
        
        // } else if(influence >= 33 && bossesDefeated == 0) {
        //     dimension = Dimension.SLIME_BOSS;
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
        Player.Instance.DamageDisabled = true;
        Player.LivingEntity.TakeDamage(new Damage {
            Type = DamageType.TRUE_DAMAGE,
            Value = 15,
        });

        Player.UICanvas.ChangeUITopState(UITopState.BlackScreen);
        Instance.Invoke(nameof(setPositionDelayed), 1.5f);
    }

    private void setPositionDelayed() {
        Player.Instance.SetPlayerPosition(AdventureManager.SpawnPosition);
        Player.Instance.DamageDisabled = false;
        Player.UICanvas.ChangeUITopState(UITopState.NotVisible);
    }
}