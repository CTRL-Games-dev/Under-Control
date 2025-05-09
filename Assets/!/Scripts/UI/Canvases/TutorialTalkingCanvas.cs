using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;


public class TutorialTalkingCanvas : MonoBehaviour
{
    [SerializeField] private RawImage _otherImage;
    [SerializeField] private CanvasGroup _middleCanvasGroup;
    private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextLocalizer _dialogueTextLocalizer;

    private bool _isTalking = false;

    private int _currentDialogueIndex = 0;
    private string _goalString = string.Empty;

    [SerializeField] private float _textSpeed = 1f;

    private Dialogue _dialogue;
    private string _otherNameKey = string.Empty;
    private FaceAnimator _otherFaceAnimator;
    private bool _blockClick = false;
    private Talkable _talkable = null;


    
    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();


        TextData.OnLanguageChanged?.RemoveListener(OnLanguageChanged);
    }


    private void OnLanguageChanged() {
        StopAllCoroutines();
        _isTalking = false;
        _dialogueText.text = _goalString;
    }



    public void SetupDialogue(Dialogue dialogue, Texture faceImage, FaceAnimator faceAnimator, string nameKey, Talkable talkable) {
        gameObject.SetActive(true);
        _dialogue = dialogue;
        _otherNameKey = nameKey;
        _otherFaceAnimator = faceAnimator;
        _otherImage.texture = faceImage;       
        _talkable = talkable;
    }



    public void ShowUI() {
        gameObject.SetActive(true);
        _currentDialogueIndex = 0;
        _dialogueText.text = string.Empty;

        _canvasGroup.DOFade(1, 0.25f * Settings.AnimationSpeed).SetEase(Ease.InOutSine).OnComplete(() => {
            _middleCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InOutSine).OnComplete(() => {
                updateDialogueBox();
            });
        });
    }


    public void HideUI() {
        _canvasGroup.DOFade(0, 0.7f * Settings.AnimationSpeed).SetEase(Ease.InOutSine).OnComplete(() => {
            gameObject.SetActive(false);
            Player.Instance.InputDisabled = false;
        });
        _middleCanvasGroup.DOFade(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
        _otherImage.DOFade(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
        
    }


    public void OnClick() {
        if (_blockClick) return;
        if (_isTalking) {
            StopAllCoroutines();
            _dialogueText.text = _goalString;
            _otherFaceAnimator.EndAnimation();
            _isTalking = false;
        } else {
            if (_currentDialogueIndex >= _dialogue.DialogueEntries.Count) {
                Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
                return;
            }
            
            _currentDialogueIndex++;
            if (_currentDialogueIndex >= _dialogue.DialogueEntries.Count) {
                _talkable.EndInteract();
                return;
            }
            updateDialogueBox();
        }
    }



    private IEnumerator animateText() {
        _isTalking = true;
        _dialogueText.text = string.Empty;

        foreach (char letter in _goalString.ToCharArray()) {
            _dialogueText.text += letter;
            float _letterInterval;
            if (letter == ' ') {
                _letterInterval = 0.05f;
            }
            else if (letter == '.' || letter == ',' || letter == ';' || letter == ':') {
                _letterInterval = 0.2f;
            }
            else if (letter == '\n') {
                _letterInterval = 0.3f;
            }
            else {
                _letterInterval = Mathf.Clamp(_textSpeed / _goalString.Length, 0.02f, 0.07f);
            }

            yield return new WaitForSeconds(_letterInterval);
        }

        if (_otherFaceAnimator != null)
            _otherFaceAnimator.EndAnimation();
        _isTalking = false;
    }



    private void updateDialogueBox() {
        _dialogueTextLocalizer.Key = _dialogue.DialogueEntries[_currentDialogueIndex].Text;

        _goalString = TextLocalizer.GetFormattedString(TextData.LocalizationTable[_dialogueTextLocalizer.Key][TextData.CurrentLanguage]);

        _otherFaceAnimator.EndAnimation();
        StopAllCoroutines();
        _dialogueText.text = string.Empty;

        _otherFaceAnimator.StartInfiniteAnimation("TALK");
        
        StartCoroutine(animateText());
    }
}
