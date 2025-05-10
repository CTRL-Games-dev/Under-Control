using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public enum TutorialState {
    None,
    Intro,
    SwordPickup,
    KillBoar,
    GoPortal,
    Fine
}

public class TutorialManager : MonoBehaviour
{
    [Serializable]
    public struct TutDialogue {
        public string TextKey;
        public TutorialState State;
    }

    [SerializeField] private LanguageChoose _languageChoose;
    [SerializeField] private List<ItemData> _starterItems;

    
    [SerializeField] private TutorialTalkingCanvas _talkingCanvas;

    [SerializeField] private FaceAnimator _faceAnimator;
    [SerializeField] private Texture _faceImage;
    [SerializeField] private string _nameKey;

    [SerializeField] private List<TutDialogue> _dialogues = new List<TutDialogue>();

    private int _dialogueIndex = 0;


    void Start() {
        Player.Animator.SetTrigger("die");
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        ItemEntity.Spawn(_starterItems[UnityEngine.Random.Range(0, _starterItems.Count -1)], 1, Player.Instance.transform.position + Vector3.up * 2 + Player.Instance.transform.forward * 2, 1, Quaternion.identity);
        _languageChoose.LanguageChooseEvent.AddListener(OnLanguageChooseEvent);
    }

    private void OnLanguageChooseEvent() {
        _talkingCanvas.SetupDialogue(_dialogues[0].TextKey);
        _talkingCanvas.ShowUI();
        Player.Animator.SetTrigger("wakeup");
    }


}
