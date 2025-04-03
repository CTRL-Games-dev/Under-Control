using Unity.Cinemachine;
using UnityEngine;

public class Talkable : MonoBehaviour, IInteractable
{
    public CinemachineCamera TalkCamera;

    public void Interact() {
        Player.Instance.UICancelEvent.AddListener(EndInteract);
        Player.Instance.SetPlayerPosition(new Vector3(2, 0, -1.5f), 0.5f, 90);
        Player.UICanvas.ChangeUIBottomState(UIBottomState.Talking);
        CameraManager.Instance.SwitchCamera(TalkCamera);
    }


    public void EndInteract() {
        CameraManager.Instance.SwitchCamera(null);
        Debug.Log("End interact");
        Player.Instance.UICancelEvent.RemoveListener(EndInteract);
    }
}

