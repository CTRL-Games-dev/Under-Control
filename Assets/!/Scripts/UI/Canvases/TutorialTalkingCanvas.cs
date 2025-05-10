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

    private string _goalString = string.Empty;

    [SerializeField] private float _textSpeed = 1f;

    private string _dialogue;
    [SerializeField] private FaceAnimator _faceAnimator;
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



    public void StartDialogue(string dialogue) {
        gameObject.SetActive(true);
        _dialogue = dialogue;
        updateDialogueBox();
    }



    private void animate() {
        gameObject.SetActive(true);
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

        if (_faceAnimator != null)
            _faceAnimator.EndAnimation();
        _isTalking = false;
    }



    private void updateDialogueBox() {
        _dialogueTextLocalizer.Key = _dialogue;

        _goalString = TextLocalizer.GetFormattedString(TextData.LocalizationTable[_dialogue][TextData.CurrentLanguage]);

        _faceAnimator.EndAnimation();
        StopAllCoroutines();
        _dialogueText.text = string.Empty;

        _faceAnimator.StartInfiniteAnimation("TALK");
        
        StartCoroutine(animateText());
    }
}
