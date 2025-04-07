using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Talkable : MonoBehaviour, IInteractable
{
    public CinemachineCamera TalkCamera;
    public Dialogue _dialogue;
    [SerializeField] private FaceAnimator _faceAnimator;
    [SerializeField] private Texture _faceImage;
    [SerializeField] private string _nameKey;


    public void Interact() {
        Player.Instance.UICancelEvent.AddListener(EndInteract);
        Player.Instance.SetPlayerPosition(new Vector3(2, 0, -1.5f), 0.5f, 90);
        CameraManager.Instance.SwitchCamera(TalkCamera);
        Player.UICanvas.StartTalking(_dialogue, _faceImage, _faceAnimator, _nameKey);
    }


    public void EndInteract() {
        CameraManager.Instance.SwitchCamera(null);
        Player.Instance.UICancelEvent.RemoveListener(EndInteract);
    }
}

