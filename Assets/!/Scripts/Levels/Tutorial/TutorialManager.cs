using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Unity.Cinemachine;
using System.Collections;

public enum TutorialState {
    None,
    Intro,
    SwordPickup,
    SwordEquip,
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
    [SerializeField] private CinemachineCamera _staringCamera;

    
    [SerializeField] private TutorialTalkingCanvas _talkingCanvas;

    [SerializeField] private List<TutDialogue> _dialogues = new List<TutDialogue>();


    [SerializeField] private Transform _enemySpawnLocation, _wall;
    [SerializeField] private GameObject _enemyPrefab;


    private Dictionary<TutorialState, string> _dialogueDictionary = new Dictionary<TutorialState, string>();
    private void Awake() {
        _dialogueDictionary.Clear();
        foreach (var dialogue in _dialogues) {
            if (!_dialogueDictionary.ContainsKey(dialogue.State)) {
                _dialogueDictionary.Add(dialogue.State, dialogue.TextKey);
            }
        }
    }


    void Start() {
        _languageChoose.LanguageChooseEvent.AddListener(OnLanguageChooseEvent);
        EventBus.ItemPlacedEvent.AddListener(OnItemPlacedEvent);

        Player.Animator.SetTrigger("die");
        CameraManager.SwitchCamera(_staringCamera);
        

    }

    private void OnLanguageChooseEvent() {
        StartCoroutine(StartDialogue(TutorialState.Intro));
    }

    private void OnItemPlacedEvent() {
        StartCoroutine(StartDialogue(TutorialState.KillBoar));
    }
    private void OnWeaponPickupEvent() {
        StartCoroutine(StartDialogue(TutorialState.SwordEquip));
    }

    private void OnEnemyDeathEvent() {
        StartCoroutine(StartDialogue(TutorialState.GoPortal));
    }

    public IEnumerator StartDialogue(TutorialState state) {
        switch (state) {
            case TutorialState.Intro:
                Player.Instance.InputDisabled = true;
                Player.Instance.LockRotation = true;
                Player.Instance.FaceAnimator.StartInfiniteAnimation("CONFUSED");
                yield return new WaitForSeconds(2f);
                _talkingCanvas.ShowUI();
                yield return new WaitForSeconds(3.5f);
                _talkingCanvas.StartDialogue(_dialogueDictionary[state]);
                yield return new WaitForSeconds(1f);
                Player.Animator.SetTrigger("wakeup");
                yield return new WaitForSeconds(5f);
                CameraManager.SwitchCamera(Player.Instance.TopDownCamera);
                yield return new WaitForSeconds(2f);
                Player.Instance.FaceAnimator.EndAnimation();
                
                Player.Instance.InputDisabled = false;
                Player.Instance.LockRotation = false;
                StartCoroutine(StartDialogue(TutorialState.SwordPickup));

                break;

            case TutorialState.SwordPickup:
                _talkingCanvas.StartDialogue(_dialogueDictionary[state]);
                yield return new WaitForSeconds(1f);
                ItemEntity weapon = ItemEntity.Spawn(_starterItems[UnityEngine.Random.Range(0, _starterItems.Count -1)], 1, Player.Instance.transform.position + Vector3.up * 2 + Player.Instance.transform.forward * 2, 1, Quaternion.identity)[0];
                weapon.PickupItemEvent.AddListener(OnWeaponPickupEvent);

                break;

            case TutorialState.SwordEquip:
                _talkingCanvas.StartDialogue(_dialogueDictionary[state]);

                break;
            
            case TutorialState.KillBoar:
                _talkingCanvas.StartDialogue(_dialogueDictionary[state]);
                LivingEntity livingEntity = Instantiate(_enemyPrefab, _enemySpawnLocation.position, Quaternion.identity).GetComponent<LivingEntity>();
                livingEntity.OnDeath.AddListener(OnEnemyDeathEvent);
                _wall.DOScaleY(0, 3f).SetEase(Ease.InOutSine);


                break;
            case TutorialState.GoPortal:
                _talkingCanvas.StartDialogue(_dialogueDictionary[state]);
                yield return new WaitForSeconds(7f);
                _talkingCanvas.HideUI();

                break;
            case TutorialState.Fine:
                _talkingCanvas.StartDialogue(_dialogueDictionary[state]);

                break;
        }
    }


}
