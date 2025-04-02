using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TalkingCanvas : MonoBehaviour, IUICanvasState
{
    [SerializeField] private RectTransform _topBar, _bottomBar;
    [SerializeField] private RawImage _playerImage, _mageImage;
    [SerializeField] private CanvasGroup _middleCanvasGroup;
    private CanvasGroup _canvasGroup, _topBarCanvasGroup, _bottomBarCanvasGroup;
    [SerializeField] private FaceAnimator _playerFaceAnimator, _mageFaceAnimator;
    [SerializeField] private TextMeshProUGUI _nameText, _dialogueText;

    private bool _isTalking = false;

    public Dialogue _dialogue;
    private int _currentDialogueIndex = 0;
    [SerializeField] private float _textSpeed = 1f;
    
    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        _topBarCanvasGroup = _topBar.GetComponent<CanvasGroup>();
        _bottomBarCanvasGroup = _bottomBar.GetComponent<CanvasGroup>();
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.B)) {
            if (_isTalking) {
                StopAllCoroutines();
                _dialogueText.text = _dialogue.dialogueEntries[_currentDialogueIndex].Text;
                _playerFaceAnimator.EndTalk();
                _mageFaceAnimator.EndTalk();
                _isTalking = false;
            } else {
                if (_currentDialogueIndex >= _dialogue.dialogueEntries.Count) {
                    Player.UICanvas.ChangeUIBottomState(UIBottomState.HUD);
                    return;
                }
                
                _currentDialogueIndex = (_currentDialogueIndex + 1) % _dialogue.dialogueEntries.Count;
                updateDialogueBox();
            }

        }
    }

    private IEnumerator animateText(DialogueEntry entry) {
        _isTalking = true;
        _dialogueText.text = string.Empty;
        float _letterInterval = Mathf.Clamp(_textSpeed / entry.Text.Length, 0.05f, 0.1f);
        Debug.Log(_letterInterval);

        foreach (char letter in entry.Text.ToCharArray()) {
            _dialogueText.text += letter;
            if (letter == ' ') {
                _letterInterval = 0.07f;
            } else if (letter == '.' || letter == ',' || letter == ';' || letter == ':') {
                _letterInterval = 0.2f;
            } else if (letter == '\n') {
                _letterInterval = 0.3f;
            } else {
                _letterInterval = Mathf.Clamp(_textSpeed / entry.Text.Length, 0.05f, 0.1f);
            }

            yield return new WaitForSeconds(_letterInterval);
        }

        _playerFaceAnimator.EndTalk();
        _mageFaceAnimator.EndTalk();
        _isTalking = false;
    }

    private void updateDialogueBox() {
        _nameText.text = _dialogue.dialogueEntries[_currentDialogueIndex].Name;
        _dialogueText.text = string.Empty;

        _playerFaceAnimator.EndTalk();
        _mageFaceAnimator.EndTalk();
        StopAllCoroutines();
        _dialogueText.text = string.Empty;

        _playerImage.DOColor(_dialogue.dialogueEntries[_currentDialogueIndex].IsPlayer ? Color.white : new Color(0.2f, 0.2f, 0.2f, 1), 0.25f);
        _mageImage.DOColor(_dialogue.dialogueEntries[_currentDialogueIndex].IsPlayer ? new Color(0.2f, 0.2f, 0.2f, 1) : Color.white, 0.25f);

        if (_dialogue.dialogueEntries[_currentDialogueIndex].IsPlayer) {
            _playerFaceAnimator.StartTalk();
        } else {
            _mageFaceAnimator.StartTalk();
        }
        StartCoroutine(animateText(_dialogue.dialogueEntries[_currentDialogueIndex]));
    }


    public void ShowUI() {
        gameObject.SetActive(true);

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
        _mageImage.DOFade(0, 0.5f).SetEase(Ease.InOutSine);
        _topBar.DOScaleY(0, 0.5f).SetEase(Ease.InOutSine);
        _topBarCanvasGroup.DOFade(0, 1f).SetEase(Ease.InOutSine);
        _bottomBar.DOScaleY(0, 0.5f).SetEase(Ease.InOutSine);
        _bottomBarCanvasGroup.DOFade(0, 1f).SetEase(Ease.InOutSine);
    }
}

