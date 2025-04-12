using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TalkingCanvas : MonoBehaviour, IUICanvasState
{
    [SerializeField] private RectTransform _topBar, _bottomBar;
    [SerializeField] private RawImage _playerImage, _otherImage;
    [SerializeField] private CanvasGroup _middleCanvasGroup;
    private CanvasGroup _canvasGroup, _topBarCanvasGroup, _bottomBarCanvasGroup;
    [SerializeField] private FaceAnimator _playerFaceAnimator;
    [SerializeField] private TextMeshProUGUI _nameText, _dialogueText;
    [SerializeField] private TextLocalizer _nameTextLocalizer, _dialogueTextLocalizer;

    private bool _isTalking = false;

    private int _currentDialogueIndex = 0;
    private string _goalString = string.Empty;

    [SerializeField] private float _textSpeed = 1f;

    private Dialogue _dialogue;
    private string _otherNameKey = string.Empty;
    private FaceAnimator _otherFaceAnimator;

    
    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        _topBarCanvasGroup = _topBar.GetComponent<CanvasGroup>();
        _bottomBarCanvasGroup = _bottomBar.GetComponent<CanvasGroup>();

        TextData.OnLanguageChanged?.RemoveListener(OnLanguageChanged);
    }

    private void OnLanguageChanged() {
        StopAllCoroutines();
        _isTalking = false;
        _dialogueText.text = _goalString;
    }


    public void SetupDialogue(Dialogue dialogue, Texture faceImage, FaceAnimator faceAnimator, string nameKey) {
        gameObject.SetActive(true);
        _dialogue = dialogue;
        _otherNameKey = nameKey;
        _otherFaceAnimator = faceAnimator;
        _otherImage.texture = faceImage;
    }



    public void ShowUI() {
        gameObject.SetActive(true);
        _currentDialogueIndex = 0;
        _nameText.text = string.Empty;
        _dialogueText.text = string.Empty;

        _canvasGroup.DOFade(1, 0.25f).SetEase(Ease.InOutSine).OnComplete(() => {
            _topBar.DOScaleY(1, 1f).SetEase(Ease.InOutSine);
            _topBarCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.InOutSine);
            
            _bottomBar.DOScaleY(1, 1f).SetEase(Ease.InOutSine).OnComplete(() => {
                _middleCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.InOutSine).OnComplete(() => {
                    updateDialogueBox();
                });
            });
            _bottomBarCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.InOutSine);
        });
    }


    public void HideUI() {

        _canvasGroup.DOFade(0, 0.7f).SetEase(Ease.InOutSine).OnComplete(() => {
            gameObject.SetActive(false);
        });
        _middleCanvasGroup.DOFade(0, 0.5f).SetEase(Ease.InOutSine);
        _otherImage.DOFade(0, 0.5f).SetEase(Ease.InOutSine);
        _topBar.DOScaleY(0, 0.5f).SetEase(Ease.InOutSine);
        _topBarCanvasGroup.DOFade(0, 1f).SetEase(Ease.InOutSine);
        _bottomBar.DOScaleY(0, 0.5f).SetEase(Ease.InOutSine);
        _bottomBarCanvasGroup.DOFade(0, 1f).SetEase(Ease.InOutSine);
    }


    public void OnClick() {
        if (_isTalking) {
            StopAllCoroutines();
            _dialogueText.text = _goalString;
            _playerFaceAnimator.EndTalk();
            _otherFaceAnimator.EndTalk();
            _isTalking = false;
        } else {
            if (_currentDialogueIndex >= _dialogue.dialogueEntries.Count) {
                Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
                return;
            }
            
            _currentDialogueIndex++;
            if (_currentDialogueIndex >= _dialogue.dialogueEntries.Count) {
                Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
                CameraManager.Instance.SwitchCamera(null);
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
                _letterInterval = Mathf.Clamp(_textSpeed / _goalString.Length, 0.03f, 0.1f);
            }

            yield return new WaitForSeconds(_letterInterval);
        }

        _playerFaceAnimator.EndTalk();
        if (_otherFaceAnimator != null)
            _otherFaceAnimator.EndTalk();
        _isTalking = false;
    }



    private void updateDialogueBox() {
        _nameTextLocalizer.Key = _dialogue.dialogueEntries[_currentDialogueIndex].IsPlayer ? "%PlayerName%" : _otherNameKey;

        _dialogueTextLocalizer.Key = _dialogue.dialogueEntries[_currentDialogueIndex].Text;

        _goalString = TextLocalizer.GetFormattedString(TextData.LocalizationTable[_dialogueTextLocalizer.Key][TextData.CurrentLanguage]);

        _playerFaceAnimator.EndTalk();
        _otherFaceAnimator.EndTalk();
        StopAllCoroutines();
        _dialogueText.text = string.Empty;

        _playerImage.DOColor(_dialogue.dialogueEntries[_currentDialogueIndex].IsPlayer ? Color.white : new Color(0.2f, 0.2f, 0.2f, 1), 0.25f);
        _otherImage.DOColor(_dialogue.dialogueEntries[_currentDialogueIndex].IsPlayer ? new Color(0.2f, 0.2f, 0.2f, 1) : Color.white, 0.25f);

        if (_dialogue.dialogueEntries[_currentDialogueIndex].IsPlayer) {
            _playerFaceAnimator.StartTalk();
        } else {
            _otherFaceAnimator.StartTalk();
        }
        StartCoroutine(animateText());
    }
}

