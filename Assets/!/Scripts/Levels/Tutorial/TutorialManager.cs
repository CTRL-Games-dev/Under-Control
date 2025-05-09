using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private LanguageChoose _languageChoose;
    [SerializeField] private List<ItemData> _starterItems;

    [SerializeField] private List<Dialogue> _dialogues = new List<Dialogue>();
    [SerializeField] private TutorialTalkingCanvas _talkingCanvas;

    [SerializeField] private FaceAnimator _faceAnimator;
    [SerializeField] private Texture _faceImage;
    [SerializeField] private string _nameKey;

    private int _dialogueIndex = 0;


    void Start() {
        Player.Animator.SetTrigger("die");
        Random.InitState(System.DateTime.Now.Millisecond);
        ItemEntity.Spawn(_starterItems[Random.Range(0, _starterItems.Count -1)], 1, Player.Instance.transform.position + Vector3.up * 2 + Player.Instance.transform.forward * 2, 1, Quaternion.identity);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            _dialogueIndex = (_dialogueIndex + 1) % _dialogues.Count;
            _talkingCanvas.SetupDialogue(_dialogues[_dialogueIndex], _faceImage, _faceAnimator, "", null);
            _talkingCanvas.ShowUI();
            

        }

    }


}
