using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;


public class TutorialTalkingCanvas : MonoBehaviour
{
    [SerializeField] private RawImage _otherImage;
    [SerializeField] private CanvasGroup _portalCanvasGroup, _bubbleCanvasGroup;
    private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextLocalizer _dialogueTextLocalizer;
    [SerializeField] private Transform _wizardTransform;


    private string _goalString = string.Empty;

    [SerializeField] private float _textSpeed = 1f;

    private string _dialogue;
    [SerializeField] private FaceAnimator _faceAnimator;

    
    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();

        TextData.OnLanguageChanged?.RemoveListener(OnLanguageChanged);
    }



    private void OnLanguageChanged() {
        StopAllCoroutines();
        _dialogueText.text = _goalString;
    }



    public void StartDialogue(string dialogue) {
        gameObject.SetActive(true);
        _dialogue = dialogue;
        _bubbleCanvasGroup.DOKill();
        _bubbleCanvasGroup.DOFade(1, 0.2f * Settings.AnimationSpeed).SetEase(Ease.InOutSine).OnComplete(() => {
            updateDialogueBox();
        });
    }


    public void HideUI() {
        _bubbleCanvasGroup.DOKill();
        _bubbleCanvasGroup.DOFade(0, 0.2f * Settings.AnimationSpeed).SetEase(Ease.InOutSine).OnComplete(() => {
            _wizardTransform.DOKill();
            _wizardTransform.DOLocalMoveY(6, 3f).SetEase(Ease.InOutCirc).OnComplete(() => {
                _portalCanvasGroup.DOKill();
                _portalCanvasGroup.DOFade(0, 1f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
            });
        });
    }

    public void ShowUI() {
        _portalCanvasGroup.DOKill();
        _portalCanvasGroup.DOFade(1, 1f * Settings.AnimationSpeed).SetEase(Ease.InOutSine).OnComplete(() => {
            _wizardTransform.DOKill();
            _wizardTransform.DOLocalMoveY(7.4f, 3f).SetEase(Ease.InOutCirc);
        });
    }



    private IEnumerator animateText() {
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
