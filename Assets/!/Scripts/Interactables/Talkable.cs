using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Talkable : MonoBehaviour, IInteractable
{
    public CinemachineCamera TalkCamera;
    public List<Dialogue> Dialogues = new List<Dialogue>();
    [SerializeField] private FaceAnimator _faceAnimator;
    [SerializeField] private Texture _faceImage;
    [SerializeField] private string _nameKey;

    public Dialogue StarterDialogue;
    private Dialogue _dialogue;
    private bool _isStarterDialogueDone = false;

    void Start() {
        if (!_isStarterDialogueDone) {
            _isStarterDialogueDone = true;
            _dialogue = StarterDialogue;
            // Player.UICanvas.StartTalking(StarterDialogue, _faceImage, _faceAnimator, _nameKey, this);
        } else {
            _dialogue = Dialogues[Random.Range(0, Dialogues.Count)];
            // Player.UICanvas.StartTalking(_dialogue, _faceImage, _faceAnimator, _nameKey, this);
        }
    }


    public void Interact() {
        Player.Instance.UICancelEvent.AddListener(EndInteract);
        Player.Instance.SetPlayerPosition(new Vector3(1.7f, 0, 1.2f), 0.5f, 114);
        CameraManager.SwitchCamera(TalkCamera);
        Player.UICanvas.StartTalking(_dialogue, _faceImage, _faceAnimator, _nameKey, this);
        Player.Instance.LockRotation = true;
    }


    public void EndInteract() {
        CameraManager.SwitchCamera(null);
        Player.Instance.UICancelEvent.RemoveListener(EndInteract);
        Player.Instance.LockRotation = false;
        Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
        Debug.Log("End Interact");
        Debug.Log(Player.Instance.LockRotation);
    }
}

