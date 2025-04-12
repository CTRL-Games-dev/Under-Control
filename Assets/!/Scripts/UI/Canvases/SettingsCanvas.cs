using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Audio;

public class SettingsCanvas : MonoBehaviour, IUICanvasState
{
    private enum SettingsState {
        Audio,
        Video,
        Controls
    }

#region Variables
    private CanvasGroup _canvasGroup;

    [Header("Buttons")]
    [SerializeField] private Image _logoImg;
    [SerializeField] private GameObject _audioButton, _videoButton, _controlsButton, _backButton;

    private RectTransform _audioButtonRect, _videoButtonRect, _controlsButtonRect, _backButtonRect;
    private CanvasGroup _audioButtonCanvasGroup, _videoButtonCanvasGroup, _controlsButtonCanvasGroup, _backButtonCanvasGroup;
    private float _audioButtonStartingY, _videoButtonStartingY, _controlsButtonStartingY, _backButtonStartingY;

    [Header("Audio")]
    [SerializeField] private GameObject _audioPanel;
    [SerializeField] private GameObject[] _audioSettings;
    private CanvasGroup _audioCanvasGroup;
    private CanvasGroup[] _audioSettingsCanvasGroups;
    
    [Header("Video")]
    [SerializeField] private GameObject _videoPanel;
    [SerializeField] private GameObject[] _videoSettings;
    private CanvasGroup _videoCanvasGroup;
    private CanvasGroup[] _videoSettingsCanvasGroups;

    [Header("Controls")]
    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private GameObject[] _controlsSettings;
    [SerializeField] private Button _bindEventButton;
    [SerializeField] private TextMeshProUGUI _bindEventText;
    [SerializeField] private TextLocalizer _bindEventTextLocalizer;

    private CanvasGroup _controlsCanvasGroup;
    private CanvasGroup[] _controlsSettingsCanvasGroups;

    private SettingsState _currentState = SettingsState.Controls;

#endregion
#region Unity Methods

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();

        _audioButtonRect = _audioButton.GetComponent<RectTransform>();
        _videoButtonRect = _videoButton.GetComponent<RectTransform>();
        _controlsButtonRect = _controlsButton.GetComponent<RectTransform>();
        _backButtonRect = _backButton.GetComponent<RectTransform>();

        _audioButtonStartingY = _audioButtonRect.anchoredPosition.y;
        _videoButtonStartingY = _videoButtonRect.anchoredPosition.y;
        _controlsButtonStartingY = _controlsButtonRect.anchoredPosition.y;
        _backButtonStartingY = _backButtonRect.anchoredPosition.y;

        _audioButtonCanvasGroup = _audioButton.GetComponent<CanvasGroup>();
        _videoButtonCanvasGroup = _videoButton.GetComponent<CanvasGroup>();
        _controlsButtonCanvasGroup = _controlsButton.GetComponent<CanvasGroup>();
        _backButtonCanvasGroup = _backButton.GetComponent<CanvasGroup>();

        _audioCanvasGroup = _audioPanel.GetComponent<CanvasGroup>();
        _audioSettingsCanvasGroups = new CanvasGroup[_audioSettings.Length];
        for (int i = 0; i < _audioSettings.Length; i++) {
            _audioSettingsCanvasGroups[i] = _audioSettings[i].GetComponent<CanvasGroup>();
        }

        _videoCanvasGroup = _videoPanel.GetComponent<CanvasGroup>();
        _videoSettingsCanvasGroups = new CanvasGroup[_videoSettings.Length];
        for (int i = 0; i < _videoSettings.Length; i++) {
            _videoSettingsCanvasGroups[i] = _videoSettings[i].GetComponent<CanvasGroup>();
        }

        _controlsCanvasGroup = _controlsPanel.GetComponent<CanvasGroup>();
        _controlsSettingsCanvasGroups = new CanvasGroup[_controlsSettings.Length];
        for (int i = 0; i < _controlsSettings.Length; i++) {
            _controlsSettingsCanvasGroups[i] = _controlsSettings[i].GetComponent<CanvasGroup>();
        }

        _bindEventButton.onClick.AddListener(() => onBindEvent());
    }

#endregion
#region Public Methods

    public void ShowUI() {
        float fadeSpeed = 0.2f;
        float moveSpeed = 0.4f;
        Ease ease = Ease.OutBack;

        gameObject.SetActive(true);

        _canvasGroup.alpha = 0;
        _logoImg.DOFade(0, 0);
        killTweens();
        resetBtnAlphas();
        resetBtnScales();
        resetPanelAlphas();
        resetBtnPositions();

        _canvasGroup.DOFade(1, 0.3f).SetUpdate(true).OnComplete(() => {
            _logoImg.DOFade(1, 0.3f).SetUpdate(true).OnComplete(() => {
                _audioButtonRect.DOAnchorPosY(_audioButtonStartingY + 20, moveSpeed).SetEase(ease).SetUpdate(true);
                _audioButtonCanvasGroup.DOFade(1, fadeSpeed).SetUpdate(true).OnComplete(() => {
                    _videoButtonRect.DOAnchorPosY(_videoButtonStartingY + 20, moveSpeed).SetEase(ease).SetUpdate(true);
                    _videoButtonCanvasGroup.DOFade(1, fadeSpeed).SetUpdate(true).OnComplete(() => {
                        _controlsButtonRect.DOAnchorPosY(_controlsButtonStartingY + 20, moveSpeed).SetEase(ease).SetUpdate(true);
                        _controlsButtonCanvasGroup.DOFade(1, fadeSpeed).SetUpdate(true).OnComplete(() => {
                            _backButtonRect.DOAnchorPosY(_backButtonStartingY + 20, moveSpeed).SetEase(ease).SetUpdate(true);
                            _backButtonCanvasGroup.DOFade(1, fadeSpeed).SetUpdate(true).OnComplete(() => {
                                openSettingsPanel(_currentState);
                            });
                        });
                    });
                });
            });
        });
    }

    public void HideUI() {
        killTweens();
        _canvasGroup.DOFade(0, 0.8f).SetUpdate(true).OnComplete(() => {
            resetBtnAlphas();
            resetBtnPositions();
            gameObject.SetActive(false);
        });
    }

#endregion
#region Help Methods

    private void killTweens() {
        _audioButtonRect.DOKill();
        _videoButtonRect.DOKill();
        _controlsButtonRect.DOKill();
        _backButtonRect.DOKill();
        _audioButtonCanvasGroup.DOKill();
        _videoButtonCanvasGroup.DOKill();
        _controlsButtonCanvasGroup.DOKill();
        _backButtonCanvasGroup.DOKill();
        _canvasGroup.DOKill();
        _logoImg.DOKill();
    }

    private void resetBtnAlphas() {
        _audioButtonCanvasGroup.alpha = 0;
        _videoButtonCanvasGroup.alpha = 0;
        _controlsButtonCanvasGroup.alpha = 0;
        _backButtonCanvasGroup.alpha = 0;
    }

    private void resetBtnScales() {
        _audioButtonRect.localScale = Vector3.one;
        _videoButtonRect.localScale = Vector3.one;
        _controlsButtonRect.localScale = Vector3.one;
        _backButtonRect.localScale = Vector3.one;
    }

    private void resetBtnPositions() {
        _audioButtonRect.anchoredPosition = new Vector2(_audioButtonRect.anchoredPosition.x, _audioButtonStartingY);
        _videoButtonRect.anchoredPosition = new Vector2(_videoButtonRect.anchoredPosition.x, _videoButtonStartingY);
        _controlsButtonRect.anchoredPosition = new Vector2(_controlsButtonRect.anchoredPosition.x, _controlsButtonStartingY);
        _backButtonRect.anchoredPosition = new Vector2(_backButtonRect.anchoredPosition.x, _backButtonStartingY);
    }

    private void resetPanelAlphas() {
        _audioCanvasGroup.alpha = 0;
        _videoCanvasGroup.alpha = 0;
        _controlsCanvasGroup.alpha = 0;
    }

    private void onBindEvent() {
        _bindEventTextLocalizer.Key = _bindEventText.text;
    }

#endregion
#region Callbacks
    
    public void OnVideoBtnClick() {
        openSettingsPanel(SettingsState.Video);
    }

    public void OnAudioBtnClick() {
        openSettingsPanel(SettingsState.Audio);
    }

    public void OnControlsBtnClick() {
        openSettingsPanel(SettingsState.Controls);
    }

    public void OnBackBtnClick() {
        Player.UICanvas.ChangeUITopState(UITopState.NotVisible);
    }

    public void OnPointerEnter(GameObject button) {
        if (button.GetComponent<RectTransform>().localScale.x > 1.05f) return;
        button.GetComponent<RectTransform>().DOScale(1.05f, 0.4f).SetEase(Ease.OutSine).SetUpdate(true);
    }

    public void OnPointerExit(GameObject button) {
        if (button.GetComponent<RectTransform>().localScale.x > 1.05f) return;
        button.GetComponent<RectTransform>().DOScale(1, 0.4f).SetEase(Ease.OutSine).SetUpdate(true);
    }

#endregion
#region Panel Management

    private void openSettingsPanel(SettingsState state) {
        if (_currentState != state) closeSettingsPanel(_currentState);
        _currentState = state;

        switch (_currentState) {
            case SettingsState.Audio:
                openAudioPanel();
                break;
            case SettingsState.Video:
                openVideoPanel();
                break;
            case SettingsState.Controls:
                openControlsPanel();
                break;
        }
    }

    private void closeSettingsPanel(SettingsState state) {
        switch (state) {
            case SettingsState.Audio:
                closeAudioPanel();
                break;
            case SettingsState.Video:
                closeVideoPanel();
                break;
            case SettingsState.Controls:
                closeControlsPanel();
                break;
        }
    }


    private void openAudioPanel() {
        _audioPanel.SetActive(true);
        _audioButtonRect.DOScale(1.1f, 0.4f).SetEase(Ease.OutSine).SetUpdate(true);
        foreach (var canvasGroup in _audioSettingsCanvasGroups) {
            canvasGroup.alpha = 0;
        }
        _audioCanvasGroup.DOFade(1, 0.5f).SetUpdate(true).OnComplete(() => {
            StartCoroutine(fadeInCanvasGroups(_audioSettingsCanvasGroups, 0.2f, 0.1f));
        });
    }

    private void openVideoPanel() {
        _videoPanel.SetActive(true);
        _videoButtonRect.DOScale(1.1f, 0.4f).SetEase(Ease.OutSine).SetUpdate(true);
        foreach (var canvasGroup in _videoSettingsCanvasGroups) {
            canvasGroup.alpha = 0;
        }
        _videoCanvasGroup.DOFade(1, 0.5f).SetUpdate(true).OnComplete(() => {
            StartCoroutine(fadeInCanvasGroups(_videoSettingsCanvasGroups, 0.2f, 0.1f));
        });
    }

    private void openControlsPanel() {
        _controlsPanel.SetActive(true);
        _controlsButtonRect.DOScale(1.1f, 0.4f).SetEase(Ease.OutSine).SetUpdate(true);
        foreach (var canvasGroup in _controlsSettingsCanvasGroups) {
            canvasGroup.alpha = 0;
        }
        _controlsCanvasGroup.DOFade(1, 0.5f).SetUpdate(true).OnComplete(() => {
            StartCoroutine(fadeInCanvasGroups(_controlsSettingsCanvasGroups, 0.2f, 0.1f));
        });
    }


    private void closeAudioPanel() {
        _audioCanvasGroup.DOKill();
        _audioButtonRect.DOScale(1, 0.4f).SetEase(Ease.OutSine).SetUpdate(true);
        StopCoroutine(fadeInCanvasGroups(_audioSettingsCanvasGroups, 0.2f, 0.1f));
        _audioCanvasGroup.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() => {
            foreach (var canvasGroup in _audioSettingsCanvasGroups) {
                canvasGroup.alpha = 0;
            }
            _audioPanel.SetActive(false);
        });
    }

    private void closeVideoPanel() {
        _videoCanvasGroup.DOKill();
        _videoButtonRect.DOScale(1, 0.4f).SetEase(Ease.OutSine).SetUpdate(true);
        StopCoroutine(fadeInCanvasGroups(_videoSettingsCanvasGroups, 0.2f, 0.1f));
        _videoCanvasGroup.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() => {
            foreach (var canvasGroup in _videoSettingsCanvasGroups) {
                canvasGroup.alpha = 0;
            }
            _videoPanel.SetActive(false);
        });
    }

    private void closeControlsPanel() {
        _controlsCanvasGroup.DOKill();
        _controlsButtonRect.DOScale(1, 0.4f).SetEase(Ease.OutSine).SetUpdate(true);
        StopCoroutine(fadeInCanvasGroups(_controlsSettingsCanvasGroups, 0.2f, 0.1f));
        _controlsCanvasGroup.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() => {
            foreach (var canvasGroup in _controlsSettingsCanvasGroups) {
                canvasGroup.alpha = 0;
            }
            _controlsPanel.SetActive(false);
        });
    }

    private IEnumerator fadeInCanvasGroups(CanvasGroup[] canvasGroup, float speed, float delay) {
        foreach (var group in canvasGroup) {
            group.DOFade(1, speed).SetUpdate(true);
            yield return new WaitForSecondsRealtime(delay);
        }
    }

#endregion
}
