using Unity.Cinemachine;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    [SerializeField] private Vector3 _spawnPosition = new();
    public static CinemachineCamera MainMenuCamera;

    private void Awake() {
        MainMenuCamera = GetComponentInChildren<CinemachineCamera>();
    }

    private void Start() {
        CameraManager.Instance.SwitchCamera(MainMenuCamera);
        Player.UICanvas.ChangeUIBottomState(UIBottomState.NotVisible);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.MainMenu);
        Player.UICanvas.ChangeUITopState(UITopState.NotVisible);
        Player.Instance.SetPlayerPosition(new Vector3(-0.7f, 0, -1.7f));
        Player.Instance.MaxCameraDistance = 7f;
        Player.Instance.SetPlayerPosition(_spawnPosition);
        Player.LivingEntity.Health = Player.LivingEntity.StartingHealth;
        Player.LivingEntity.Mana = Player.LivingEntity.StartingMana;
        Player.Animator.SetTrigger("live");
        Player.Instance.InputDisabled = true;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            Player.UICanvas.ChangeUIMiddleState(UIMiddleState.MainMenu);
            CameraManager.Instance.SwitchCamera(MainMenuCamera);
        }
    }
}