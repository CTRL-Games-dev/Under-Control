using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class PauseCanvas : MonoBehaviour, IUICanvasState
{
    [SerializeField] private Image _bgImage;
    [SerializeField] private GameObject _blackBar, _resumeButton, _optionsButton, _exitButton;
    private RectTransform _blackBarRect, _resumeRect, _optionsRect, _exitRect;
    private float _resumeBtnStartingY, _optionsBtnStartingY, _exitBtnStartingY;
    private CanvasGroup _canvasGroup, _resumeGroup, _optionsGroup, _exitGroup;
    private TweenParams _tweenParams = new TweenParams().SetUpdate(true).SetEase(Ease.OutBack);

#region Unity Methods
    private void Awake() {
        _blackBarRect = _blackBar.GetComponent<RectTransform>();

        _resumeRect = _resumeButton.GetComponent<RectTransform>();
        _optionsRect = _optionsButton.GetComponent<RectTransform>();
        _exitRect = _exitButton.GetComponent<RectTransform>();

        _canvasGroup = GetComponent<CanvasGroup>();
        _resumeGroup = _resumeButton.GetComponent<CanvasGroup>();
        _optionsGroup = _optionsButton.GetComponent<CanvasGroup>();
        _exitGroup = _exitButton.GetComponent<CanvasGroup>();
        
        _resumeBtnStartingY = _resumeRect.anchoredPosition.y;
        _optionsBtnStartingY = _optionsRect.anchoredPosition.y;
        _exitBtnStartingY = _exitRect.anchoredPosition.y;
    }
    #endregion
    #region Public Methods
    public void ShowUI() {
        float fadeSpeed = 0.2f * Settings.AnimationSpeed;
        float moveSpeed = 0.4f * Settings.AnimationSpeed;
        
        gameObject.SetActive(true);
        killTweens();
        resetBtnAlphas();
        resetBtnPositions();

        StartCoroutine(slowdownTime());
        _canvasGroup.alpha = 1;
        _bgImage.DOFade(1f, 0.8f * Settings.AnimationSpeed).SetUpdate(true);

        _blackBarRect.DOAnchorPosY(-1080, 0.3f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).SetUpdate(true).OnComplete(() => {
            _resumeRect.DOAnchorPosY(_resumeBtnStartingY + 20, moveSpeed).SetAs(_tweenParams);
            _resumeGroup.DOFade(1, fadeSpeed).SetAs(_tweenParams).OnComplete(() => {
                _optionsRect.DOAnchorPosY(_optionsBtnStartingY + 20, moveSpeed).SetAs(_tweenParams);
                _optionsGroup.DOFade(1, fadeSpeed).SetAs(_tweenParams).OnComplete(() => {
                    _exitRect.DOAnchorPosY(_exitBtnStartingY + 20, moveSpeed).SetAs(_tweenParams);
                    _exitGroup.DOFade(1, fadeSpeed).SetAs(_tweenParams);
                });
            });
        });
    }

    public void HideUI() {
        hideUI();
    }

    


#endregion
#region Private Methods
    private Tween hideUI() {
        gameObject.SetActive(true);
        StopCoroutine(slowdownTime());
        StartCoroutine(speedupTime());
        killTweens();

        _bgImage.DOFade(0f, 0.8f * Settings.AnimationSpeed).SetUpdate(true);
        return _blackBarRect.DOAnchorPosY(0, 0.6f * Settings.AnimationSpeed).SetEase(Ease.InQuint).SetUpdate(true).OnComplete(() => {
            resetBtnPositions();
            resetBtnAlphas();
            StopCoroutine(speedupTime());
            Time.timeScale = 1;
            gameObject.SetActive(false);
        });
    }
    private void killTweens() {
        _blackBarRect.DOKill();
        _canvasGroup.DOKill();
        _bgImage.DOKill();
        _resumeRect.DOKill();
        _resumeGroup.DOKill();
        _optionsRect.DOKill();
        _optionsGroup.DOKill();
        _exitRect.DOKill();
        _exitGroup.DOKill();
    }

    private void resetBtnPositions() {
        _resumeRect.anchoredPosition = new Vector2(_resumeRect.anchoredPosition.x, _resumeBtnStartingY);
        _optionsRect.anchoredPosition = new Vector2(_optionsRect.anchoredPosition.x, _optionsBtnStartingY);
        _exitRect.anchoredPosition = new Vector2(_exitRect.anchoredPosition.x, _exitBtnStartingY);
    }

    private void resetBtnAlphas() {
        _resumeGroup.alpha = 0;
        _optionsGroup.alpha = 0;
        _exitGroup.alpha = 0;
    }

    private IEnumerator slowdownTime() {
        Time.timeScale = 1;
        for (int i = 0; i < 100; i++) {
            Time.timeScale = Mathf.Clamp(Time.timeScale - 0.01f, 0f, 1f);
            yield return new WaitForSecondsRealtime(0.005f);
        }
        Time.timeScale = 0;
    }

    private IEnumerator speedupTime() {
        Time.timeScale = 0.01f;
        for (int i = 0; i < 100; i++) {
            Time.timeScale = Mathf.Clamp(Time.timeScale + 0.01f, 0f, 1f);
            yield return new WaitForSecondsRealtime(0.005f);
        }
        Time.timeScale = 1;
    }


#endregion
#region Button Clicks
    public void OnResumeBtnClick() {
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
    }  

    public void OnSettingsBtnClick() {
        Player.UICanvas.ChangeUITopState(UITopState.Settings);
    }
    public void OnExitBtnClick() {
        hideUI().OnComplete(() => {
            if(GameManager.Instance.CurrentDimension == Dimension.HUB) {
                Player.UICanvas.ChangeUIMiddleState(UIMiddleState.MainMenu);
            } else {
                GameManager.Instance.ShowMainMenu = true;
                Player.LivingEntity.OnDeath?.Invoke();
            }
        });
    }

    public void OnPointerEnter(GameObject button) {
        button.GetComponent<RectTransform>().DOScale(1.05f, 0.4f * Settings.AnimationSpeed).SetEase(Ease.OutSine).SetUpdate(true);
    }

    public void OnPointerExit(GameObject button) {
        button.GetComponent<RectTransform>().DOScale(1, 0.4f * Settings.AnimationSpeed).SetEase(Ease.OutSine).SetUpdate(true);
    }

#endregion
}
