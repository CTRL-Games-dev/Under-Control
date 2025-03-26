using Unity.Cinemachine;
using UnityEngine;

public class HubManager : MonoBehaviour, ILevelManager
{
    public static CinemachineCamera MainMenuCamera;

    private void Awake() {
        MainMenuCamera = GetComponentInChildren<CinemachineCamera>();
    }

    private void Start() {
        CameraManager.Instance.SwitchCamera(MainMenuCamera);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.MainMenu);
    }
}