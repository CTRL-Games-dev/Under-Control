using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image _logoImage, _bgImage;
    [SerializeField] private GameObject _continueButton, _newGameButton, _optionsButton, _creditsButton, _exitButton;
    private RectTransform _continueRect, _newGameRect, _optionsRect, _creditsRect, _exitRect;
    private float _continueBtnStartingX, _newGameBtnStartingX, _optionsBtnStartingX, _creditsBtnStartingX, _exitBtnStartingX;
    private CanvasGroup _canvasGroup, _continueGroup, _newGameGroup, _optionsGroup, _creditsGroup, _exitGroup;

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();

        _continueRect = _continueButton.GetComponent<RectTransform>();
        _newGameRect = _newGameButton.GetComponent<RectTransform>();
        _optionsRect = _optionsButton.GetComponent<RectTransform>();
        _creditsRect = _creditsButton.GetComponent<RectTransform>();
        _exitRect = _exitButton.GetComponent<RectTransform>();

        _continueGroup = _continueButton.GetComponent<CanvasGroup>();
        _newGameGroup = _newGameButton.GetComponent<CanvasGroup>();
        _optionsGroup = _optionsButton.GetComponent<CanvasGroup>();
        _creditsGroup = _creditsButton.GetComponent<CanvasGroup>();
        _exitGroup = _exitButton.GetComponent<CanvasGroup>();
        
        _continueBtnStartingX = _continueRect.anchoredPosition.x;
        _newGameBtnStartingX = _newGameRect.anchoredPosition.x;
        _optionsBtnStartingX = _optionsRect.anchoredPosition.x;
        _creditsBtnStartingX = _creditsRect.anchoredPosition.x;
        _exitBtnStartingX = _exitRect.anchoredPosition.x;
    }

    public void PlayGame() {
        Debug.Log("");
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void OpenMenu() {
        if (HubManager.MainMenuCamera != null) CameraManager.Instance.SwitchCamera(HubManager.MainMenuCamera);

        gameObject.SetActive(true);

        _canvasGroup.alpha = 1;
        _bgImage.DOFade(230.0f / 255.0f, 1f).OnComplete(() => {
            animateButtons();
        });
    }

    public void CloseMenu() {
        killTweens();
        CameraManager.Instance.SwitchCamera(CameraManager.Instance.PlayerTopDownCamera);

        _canvasGroup.DOFade(0, 1f).OnComplete(() => {
            _bgImage.DOFade(0, 0);
            _logoImage.DOFade(0, 0);
            resetBtnAlphas();
            gameObject.SetActive(false);
        });
    }

    private void animateButtons() {
        Ease ease = Ease.OutQuint;
        float moveSpeed = 0.9f;
        float fadeSpeed = 0.2f;

        killTweens();
        resetBtnAlphas();
        resetBtnPositions();

        _logoImage.DOFade(1, 0.6f).SetEase(Ease.InSine).OnComplete(() => {
            _continueRect.DOAnchorPosX(_continueBtnStartingX -50, moveSpeed).SetEase(ease);
            _continueGroup.DOFade(1, fadeSpeed).OnComplete(() => {
                _newGameRect.DOAnchorPosX(_newGameBtnStartingX -50, moveSpeed).SetEase(ease);
                _newGameGroup.DOFade(1, fadeSpeed).OnComplete(() => {
                    _optionsRect.DOAnchorPosX(_optionsBtnStartingX -50, moveSpeed).SetEase(ease);
                    _optionsGroup.DOFade(1, fadeSpeed).OnComplete(() => {
                        _creditsRect.DOAnchorPosX(_creditsBtnStartingX -50, moveSpeed).SetEase(ease);
                        _creditsGroup.DOFade(1, fadeSpeed).OnComplete(() => {
                            _exitRect.DOAnchorPosX(_exitBtnStartingX -50, moveSpeed).SetEase(ease);
                            _exitGroup.DOFade(1, 0.2f);
                        });
                    });
                });
            });
        });
    }

    private void killTweens() {
        _canvasGroup.DOKill();
        _bgImage.DOKill();
        _logoImage.DOKill();
        _continueRect.DOKill();
        _continueGroup.DOKill();
        _newGameRect.DOKill();
        _newGameGroup.DOKill();
        _optionsRect.DOKill();
        _optionsGroup.DOKill();
        _creditsRect.DOKill();
        _creditsGroup.DOKill();
        _exitRect.DOKill();
        _exitGroup.DOKill();
    }

    private void resetBtnAlphas() {
        _continueGroup.alpha = 0;
        _newGameGroup.alpha = 0;
        _optionsGroup.alpha = 0;
        _creditsGroup.alpha = 0;
        _exitGroup.alpha = 0;
    }

    private void resetBtnPositions() {
        _continueRect.anchoredPosition = new Vector2(_continueBtnStartingX +50, _continueRect.anchoredPosition.y);
        _newGameRect.anchoredPosition = new Vector2(_newGameBtnStartingX +50, _newGameRect.anchoredPosition.y);
        _optionsRect.anchoredPosition = new Vector2(_optionsBtnStartingX +50, _optionsRect.anchoredPosition.y);
        _creditsRect.anchoredPosition = new Vector2(_creditsBtnStartingX +50, _creditsRect.anchoredPosition.y);
        _exitRect.anchoredPosition = new Vector2(_exitBtnStartingX +50, _exitRect.anchoredPosition.y);
    }

    public void OnContinueGameBtnClick() {
        UICanvas.Instance.CloseUIState(UIState.MainMenu);
        UICanvas.Instance.OpenUIState(UIState.NotVisible);
    }

    public void OnNewGameBtnClick() {
        Debug.Log("New Game");
    }

    public void OnSettingsBtnClick() {
        UICanvas.Instance.OpenSettingsScreen();
    }

    public void OnCreditsBtnClick() {
        Debug.Log("BY Ctrl Games");
    }

    public void OnExitBtnClick() {
        Debug.Log("exit");
    }

    public void OnPointerEnter(GameObject button) {
        button.GetComponent<RectTransform>().DOScale(1.1f, 0.4f).SetEase(Ease.OutSine);
    }

    public void OnPointerExit(GameObject button) {
        button.GetComponent<RectTransform>().DOScale(1, 0.4f).SetEase(Ease.OutSine);
    }
}
