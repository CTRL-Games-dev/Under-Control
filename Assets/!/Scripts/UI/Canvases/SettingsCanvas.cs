using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class SettingsCanvas : MonoBehaviour, IUICanvasState
{
    private enum SettingsState {
        General,
        Audio,
        Video,
        Controls
    }

#region Variables
    private CanvasGroup _canvasGroup;

    [Header("Buttons")]
    [SerializeField] private Image _logoImg;
    [SerializeField] private GameObject _generalButton, _audioButton, _videoButton, _controlsButton, _backButton;

    private RectTransform _generalButtonRect, _videoButtonRect, _controlsButtonRect, _backButtonRect;
    private CanvasGroup _generalButtonCanvasGroup, _videoButtonCanvasGroup, _controlsButtonCanvasGroup, _backButtonCanvasGroup;
    private float _generalButtonStartingY, _videoButtonStartingY, _controlsButtonStartingY, _backButtonStartingY;

    [Header("General")]
    [SerializeField] private GameObject _generalPanel;
    [SerializeField] private GameObject[] _generalSettings;
    private CanvasGroup _generalCanvasGroup;
    private CanvasGroup[] _generalSettingsCanvasGroups;

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

    private SettingsState _currentState = SettingsState.General;

#endregion
#region Unity Methods

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();

        _generalButtonRect = _generalButton.GetComponent<RectTransform>();
        _videoButtonRect = _videoButton.GetComponent<RectTransform>();
        _controlsButtonRect = _controlsButton.GetComponent<RectTransform>();
        _backButtonRect = _backButton.GetComponent<RectTransform>();

        _generalButtonStartingY = _generalButtonRect.anchoredPosition.y;
        _videoButtonStartingY = _videoButtonRect.anchoredPosition.y;
        _controlsButtonStartingY = _controlsButtonRect.anchoredPosition.y;
        _backButtonStartingY = _backButtonRect.anchoredPosition.y;

        _generalButtonCanvasGroup = _generalButton.GetComponent<CanvasGroup>();
        _videoButtonCanvasGroup = _videoButton.GetComponent<CanvasGroup>();
        _controlsButtonCanvasGroup = _controlsButton.GetComponent<CanvasGroup>();
        _backButtonCanvasGroup = _backButton.GetComponent<CanvasGroup>();

        _generalCanvasGroup = _generalPanel.GetComponent<CanvasGroup>();
        _generalSettingsCanvasGroups = new CanvasGroup[_generalSettings.Length];
        for (int i = 0; i < _generalSettings.Length; i++) {
            _generalSettingsCanvasGroups[i] = _generalSettings[i].GetComponent<CanvasGroup>();
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
        float fadeSpeed = 0.1f * Settings.AnimationSpeed;
        float moveSpeed = 0.2f * Settings.AnimationSpeed;
        Ease ease = Ease.OutBack;

        gameObject.SetActive(true);

        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 0;
        _logoImg.DOFade(0, 0);
        killTweens();
        resetBtnAlphas();
        resetBtnScales();
        resetPanelAlphas();
        resetBtnPositions();

        _canvasGroup.DOFade(1, 0.15f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
            _logoImg.DOFade(1, 0.15f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
                _generalButtonRect.DOAnchorPosY(_generalButtonStartingY + 20, moveSpeed).SetEase(ease).SetUpdate(true);
                _generalButtonCanvasGroup.DOFade(1, fadeSpeed).SetUpdate(true).OnComplete(() => {

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
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        
        killTweens();
        _canvasGroup.DOFade(0, 0.3f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
            resetBtnAlphas();
            resetBtnPositions();
            gameObject.SetActive(false);
        });
    }

#endregion
#region Help Methods

    private void killTweens() {
        _generalButtonRect.DOKill();
        _videoButtonRect.DOKill();
        _controlsButtonRect.DOKill();
        _backButtonRect.DOKill();
        _generalButtonCanvasGroup.DOKill();
        _videoButtonCanvasGroup.DOKill();
        _controlsButtonCanvasGroup.DOKill();
        _backButtonCanvasGroup.DOKill();
        _canvasGroup.DOKill();
        _logoImg.DOKill();
    }

    private void resetBtnAlphas() {
        _generalButtonCanvasGroup.alpha = 0;
        _videoButtonCanvasGroup.alpha = 0;
        _controlsButtonCanvasGroup.alpha = 0;
        _backButtonCanvasGroup.alpha = 0;
    }

    private void resetBtnScales() {
        _generalButtonRect.localScale = Vector3.one;
        _videoButtonRect.localScale = Vector3.one;
        _controlsButtonRect.localScale = Vector3.one;
        _backButtonRect.localScale = Vector3.one;
    }

    private void resetBtnPositions() {
        _generalButtonRect.anchoredPosition = new Vector2(_generalButtonRect.anchoredPosition.x, _generalButtonStartingY);
        _videoButtonRect.anchoredPosition = new Vector2(_videoButtonRect.anchoredPosition.x, _videoButtonStartingY);
        _controlsButtonRect.anchoredPosition = new Vector2(_controlsButtonRect.anchoredPosition.x, _controlsButtonStartingY);
        _backButtonRect.anchoredPosition = new Vector2(_backButtonRect.anchoredPosition.x, _backButtonStartingY);
    }

    private void resetPanelAlphas() {
        _videoCanvasGroup.alpha = 0;
        _controlsCanvasGroup.alpha = 0;
    }

    private void onBindEvent() {
        _bindEventTextLocalizer.Key = _bindEventText.text;
    }

#endregion
#region Callbacks

    public void OnGeneralBtnClick() {
        openSettingsPanel(SettingsState.General);
    }
    
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
        button.GetComponent<RectTransform>().DOScale(1.05f, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutSine).SetUpdate(true);
    }

    public void OnPointerExit(GameObject button) {
        if (button.GetComponent<RectTransform>().localScale.x > 1.05f) return;
        button.GetComponent<RectTransform>().DOScale(1, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutSine).SetUpdate(true);
    }

#endregion
#region Panel Management

    private void openSettingsPanel(SettingsState state) {
        if (_currentState != state) closeSettingsPanel(_currentState);
        _currentState = state;

        switch (_currentState) {
            case SettingsState.General:
                openGeneralPanel();
                break;
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
            case SettingsState.General:
                closeGeneralPanel();
                break;
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

    private void openGeneralPanel() {
        _generalPanel.SetActive(true);
        _generalCanvasGroup.interactable = true;
        _generalCanvasGroup.blocksRaycasts = true;
        _generalButtonRect.DOScale(1.1f, 0.4f * Settings.AnimationSpeed).SetEase(Ease.OutSine).SetUpdate(true);
        foreach (var canvasGroup in _generalSettingsCanvasGroups) {
            canvasGroup.alpha = 0;
        }
        _generalCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
            StartCoroutine(fadeInCanvasGroups(_generalSettingsCanvasGroups, 0.1f * Settings.AnimationSpeed, 0.05f * Settings.AnimationSpeed));
        });
    }


    private void openAudioPanel() {
        _audioPanel.SetActive(true);
        _audioCanvasGroup.interactable = true;
        _audioCanvasGroup.blocksRaycasts = true;
        foreach (var canvasGroup in _audioSettingsCanvasGroups) {
            canvasGroup.alpha = 0;
        }
        _audioCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
            StartCoroutine(fadeInCanvasGroups(_audioSettingsCanvasGroups, 0.1f * Settings.AnimationSpeed, 0.05f * Settings.AnimationSpeed));
        });
    }

    private void openVideoPanel() {
        _videoPanel.SetActive(true);
        _videoCanvasGroup.interactable = true;
        _videoCanvasGroup.blocksRaycasts = true;
        _videoButtonRect.DOScale(1.1f, 0.4f * Settings.AnimationSpeed).SetEase(Ease.OutSine).SetUpdate(true);
        foreach (var canvasGroup in _videoSettingsCanvasGroups) {
            canvasGroup.alpha = 0;
        }
        _videoCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
            StartCoroutine(fadeInCanvasGroups(_videoSettingsCanvasGroups, 0.1f * Settings.AnimationSpeed, 0.05f * Settings.AnimationSpeed));
        });
    }

    private void openControlsPanel() {
        _controlsPanel.SetActive(true);
        _controlsCanvasGroup.interactable = true;
        _controlsCanvasGroup.blocksRaycasts = true;
        _controlsButtonRect.DOScale(1.1f, 0.4f * Settings.AnimationSpeed).SetEase(Ease.OutSine).SetUpdate(true);
        foreach (var canvasGroup in _controlsSettingsCanvasGroups) {
            canvasGroup.alpha = 0;
        }
        _controlsCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
            StartCoroutine(fadeInCanvasGroups(_controlsSettingsCanvasGroups, 0.1f * Settings.AnimationSpeed, 0.05f * Settings.AnimationSpeed));
        });
    }


    private void closeAudioPanel() {
        _audioCanvasGroup.DOKill();
        StopCoroutine(fadeInCanvasGroups(_audioSettingsCanvasGroups, 0.2f, 0.1f));
        _audioCanvasGroup.DOFade(0, 0.25f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
            foreach (var canvasGroup in _audioSettingsCanvasGroups) {
                canvasGroup.alpha = 0;
            }
            _audioCanvasGroup.interactable = false;
            _audioCanvasGroup.blocksRaycasts = false;
            _audioPanel.SetActive(false);
        });
    }

    private void closeVideoPanel() {
        _videoCanvasGroup.DOKill();
        _videoButtonRect.DOScale(1, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutSine).SetUpdate(true);
        StopCoroutine(fadeInCanvasGroups(_videoSettingsCanvasGroups, 0.1f, 0.05f));
        _videoCanvasGroup.DOFade(0, 0.25f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
            foreach (var canvasGroup in _videoSettingsCanvasGroups) {
                canvasGroup.alpha = 0;
            }
            _videoCanvasGroup.interactable = false;
            _videoCanvasGroup.blocksRaycasts = false;
            _videoPanel.SetActive(false);
        });
    }

    private void closeControlsPanel() {
        _controlsCanvasGroup.DOKill();
        _controlsButtonRect.DOScale(1, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutSine).SetUpdate(true);
        StopCoroutine(fadeInCanvasGroups(_controlsSettingsCanvasGroups, 0.1f, 0.05f));
        _controlsCanvasGroup.DOFade(0, 0.25f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
            foreach (var canvasGroup in _controlsSettingsCanvasGroups) {
                canvasGroup.alpha = 0;
            }
            _controlsCanvasGroup.interactable = false;
            _controlsCanvasGroup.blocksRaycasts = false;
            _controlsPanel.SetActive(false);
        });
    }

    private void closeGeneralPanel() {
        _generalCanvasGroup.DOKill();
        _generalButtonRect.DOScale(1, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutSine).SetUpdate(true);
        StopCoroutine(fadeInCanvasGroups(_generalSettingsCanvasGroups, 0.1f, 0.05f));
        _generalCanvasGroup.DOFade(0, 0.25f * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
            foreach (var canvasGroup in _generalSettingsCanvasGroups) {
                canvasGroup.alpha = 0;
            }
            _generalCanvasGroup.interactable = false;
            _generalCanvasGroup.blocksRaycasts = false;
            _generalPanel.SetActive(false);
        });
    }


    private IEnumerator fadeInCanvasGroups(CanvasGroup[] canvasGroup, float speed, float delay) {
        foreach (var group in canvasGroup) {
            group.DOFade(1, speed * Settings.AnimationSpeed).SetUpdate(true);
            yield return new WaitForSecondsRealtime(delay * Settings.AnimationSpeed);
        }
    }

#endregion
}
