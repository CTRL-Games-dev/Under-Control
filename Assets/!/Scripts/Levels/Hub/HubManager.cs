using Unity.Cinemachine;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public static CinemachineCamera MainMenuCamera;

    private void Awake() {
        MainMenuCamera = GetComponentInChildren<CinemachineCamera>();
    }

    private void Start() {
        CameraManager.Instance.SwitchCamera(MainMenuCamera);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.MainMenu);
        Player.Instance.SetPlayerPosition(new Vector3(-0.7f, 0, -1.7f));
        Player.Instance.MaxCameraDistance = 7f;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            Player.UICanvas.ChangeUIMiddleState(UIMiddleState.MainMenu);
            CameraManager.Instance.SwitchCamera(MainMenuCamera);
        }
    }
}