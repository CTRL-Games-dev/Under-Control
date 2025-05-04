using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class HubManager : MonoBehaviour
{
    [SerializeField] private Vector3 _spawnPosition = new();
    public static CinemachineCamera MainMenuCamera;
    [SerializeField] private Transform _eyeTransform, _cylinderTransform;

    private void Awake() {
        MainMenuCamera = GetComponentInChildren<CinemachineCamera>();
    }

    private void Start() {
        CameraManager.SwitchCamera(MainMenuCamera);
        Player.UICanvas.ChangeUIBottomState(UIBottomState.NotVisible);
        if (GameManager.Instance.ShowMainMenu) {
            Player.UICanvas.ChangeUIMiddleState(UIMiddleState.MainMenu);
        } else {
            Player.UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
            Player.Instance.PlayRespawnAnimation();
        }
        Player.UICanvas.ChangeUITopState(UITopState.NotVisible);
        Player.Instance.MaxCameraDistance = 7f;
        Player.Instance.UpdateDisabled = true;
        Player.Instance.gameObject.transform.position = _spawnPosition;
        Player.LivingEntity.Health = Player.LivingEntity.StartingHealth;
        Player.LivingEntity.Mana = Player.LivingEntity.StartingMana;
        Player.Instance.HasPlayerDied = false;
        Player.LivingEntity.HasDied = false;
        Player.Instance.EvolutionPoints++;

        GameManager.Instance.ResetInfluence();
        GameManager.Instance.ResetCards();
        // Player.Animator.SetTrigger("live");
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.F4)) {
            Player.Instance.PlayRespawnAnimation();
        }
    }

    
}