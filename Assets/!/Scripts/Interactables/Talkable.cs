using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class Talkable : MonoBehaviour, IInteractable
{
    public CinemachineCamera TalkCamera;
    public List<Dialogue> Dialogues = new List<Dialogue>();
    [SerializeField] private FaceAnimator _faceAnimator;
    [SerializeField] private Texture _faceImage;
    [SerializeField] private string _nameKey;
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _talkOffset;
    [SerializeField] private float _talkRotation;
    public Dialogue StarterDialogue;
    private Dialogue _dialogue;
    private bool _interacted = false;

    [SerializeField] private Transform _exclamationMark;


    void Start() {
        if (!GameManager.Instance.IsStarterDialogueOver) {
            _dialogue = StarterDialogue;
        } else {
            Random.InitState(System.DateTime.Now.Millisecond);
            _dialogue = Dialogues[Random.Range(0, Dialogues.Count)];
        }
        AnimateMark();
    }


    public void Interact() {
        Player.Instance.UICancelEvent.AddListener(EndInteract);
        Player.UICanvas.StartTalking(_dialogue, _faceImage, _faceAnimator, _nameKey, this);
        _interacted = true;
        CameraManager.SwitchCamera(TalkCamera);
        Player.Instance.SetPlayerPosition(_target.position + _talkOffset, 0.5f, _talkRotation);
        Player.Instance.LockRotation = true;
        
        _exclamationMark?.DOComplete();
        _exclamationMark?.DOKill();
        _exclamationMark?.gameObject.SetActive(false);

    }


    public void EndInteract() {
        if (_dialogue.Equals(StarterDialogue)) {
            GameManager.Instance.IsStarterDialogueOver = true;
        }
        CameraManager.SwitchCamera(null);
        Player.Instance.UICancelEvent.RemoveListener(EndInteract);
        Player.Instance.LockRotation = false;
        Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
        
    }

    private void AnimateMark() {
        _exclamationMark?.DOMoveY(2.2f, 0.8f).SetDelay(1f).SetEase(Ease.OutBack);
        _exclamationMark?.DOScale(0.8f, 0.8f).SetDelay(1f).SetEase(Ease.OutBack).OnComplete(() => {
            _exclamationMark?.DOScale(0.9f, 0.2f).OnComplete(() => {
                _exclamationMark?.DOScale(0.8f, 0.2f).SetDelay(0.2f);
            });
            _exclamationMark?.DORotate(new Vector3(0, 180, 0), 0.3f, RotateMode.Fast).SetEase(Ease.InOutCirc).OnComplete(() => {
                _exclamationMark?.DORotate(new Vector3(0, 360, 0), 0.3f, RotateMode.Fast).SetEase(Ease.InOutCirc).OnComplete(() => {
                    _exclamationMark.rotation = Quaternion.identity;
        
                    _exclamationMark?.DOMoveY(1.9f, 0.8f).SetDelay(0.5f).SetEase(Ease.OutBack);
                    _exclamationMark?.DOScale(0.3f, 0.8f).SetDelay(0.5f).SetEase(Ease.OutBack).OnComplete(() => {
                        if (!_interacted) AnimateMark();
                        else {
                            _exclamationMark?.DOComplete();
                            _exclamationMark?.DOKill();
                            _exclamationMark?.gameObject.SetActive(false);
                        }
                    });
                });
            });
        });
    }

}

