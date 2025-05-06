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
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private GameObject _confirmButton;

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
        
        _topBarCanvasGroup = _topBar.GetComponent<CanvasGroup>();
        _bottomBarCanvasGroup = _bottomBar.GetComponent<CanvasGroup>();

        TextData.OnLanguageChanged?.RemoveListener(OnLanguageChanged);
    }


    private void OnLanguageChanged() {
        StopAllCoroutines();
        _isTalking = false;
        _dialogueText.text = _goalString;
    }


    public void OnBtnConfirmClick() {
        FormattedStrings.PlayerName = _inputField.text;
        _blockClick = false;
        _inputField.gameObject.SetActive(false);
        _confirmButton.SetActive(false);
        _inputField.interactable = false;
        _inputField.DeactivateInputField();
        OnClick();
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
        _nameText.text = string.Empty;
        _dialogueText.text = string.Empty;

        _canvasGroup.DOFade(1, 0.25f * Settings.AnimationSpeed).SetEase(Ease.InOutSine).OnComplete(() => {
            _topBar.DOScaleY(1, 1f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
            _topBarCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
            
            _bottomBar.DOScaleY(1, 1f * Settings.AnimationSpeed).SetEase(Ease.InOutSine).OnComplete(() => {
                _middleCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InOutSine).OnComplete(() => {
                    updateDialogueBox();
                });
            });
            _bottomBarCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
        });
    }


    public void HideUI() {
        _canvasGroup.DOFade(0, 0.7f * Settings.AnimationSpeed).SetEase(Ease.InOutSine).OnComplete(() => {
            gameObject.SetActive(false);
            Player.Instance.InputDisabled = false;
        });
        _middleCanvasGroup.DOFade(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
        _otherImage.DOFade(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
        _topBar.DOScaleY(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
        _topBarCanvasGroup.DOFade(0, 1f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
        _bottomBar.DOScaleY(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
        _bottomBarCanvasGroup.DOFade(0, 1f * Settings.AnimationSpeed).SetEase(Ease.InOutSine);
        
    }


    public void OnClick() {
        if (_blockClick) return;
        if (_isTalking) {
            StopAllCoroutines();
            _dialogueText.text = _goalString;
            _playerFaceAnimator.EndAnimation();
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

        _playerFaceAnimator.EndAnimation();
        if (_otherFaceAnimator != null)
            _otherFaceAnimator.EndAnimation();
        _isTalking = false;
    }



    private void updateDialogueBox() {
        _nameTextLocalizer.Key = _dialogue.DialogueEntries[_currentDialogueIndex].IsPlayer ? "%PlayerName%" : _otherNameKey;

        _dialogueTextLocalizer.Key = _dialogue.DialogueEntries[_currentDialogueIndex].Text;

        _goalString = TextLocalizer.GetFormattedString(TextData.LocalizationTable[_dialogueTextLocalizer.Key][TextData.CurrentLanguage]);

        _playerFaceAnimator.EndAnimation();
        _otherFaceAnimator.EndAnimation();
        StopAllCoroutines();
        _dialogueText.text = string.Empty;

        _playerImage.DOColor(_dialogue.DialogueEntries[_currentDialogueIndex].IsPlayer ? Color.white : new Color(0.2f, 0.2f, 0.2f, 1), 0.25f * Settings.AnimationSpeed);
        _otherImage.DOColor(_dialogue.DialogueEntries[_currentDialogueIndex].IsPlayer ? new Color(0.2f, 0.2f, 0.2f, 1) : Color.white, 0.25f * Settings.AnimationSpeed);

        if (_dialogue.DialogueEntries[_currentDialogueIndex].IsPlayer) {
            _playerFaceAnimator.StartInfiniteAnimation("TALK");
        } else {
            _otherFaceAnimator.StartInfiniteAnimation("TALK");
        }
        StartCoroutine(animateText());

  
        if (_dialogue.DialogueEntries[_currentDialogueIndex].IsInputField) {
            _inputField.gameObject.SetActive(true);
            _confirmButton.SetActive(true);
            _inputField.interactable = true;
            _inputField.Select();
            _inputField.ActivateInputField();
            _blockClick = true;
        }
            
    }
}

