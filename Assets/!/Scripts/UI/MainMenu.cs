using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _titleImg, _playBtn, _quitBtn;
    [SerializeField] private CinemachineCamera _mainMenuCamera;
    private RectTransform _titleImgRect, _playBtnRect, _quitBtnRect;
    private Image _img;
    

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();   
        _titleImgRect = _titleImg.GetComponent<RectTransform>();
        _playBtnRect = _playBtn.GetComponent<RectTransform>();
        _quitBtnRect = _quitBtn.GetComponent<RectTransform>();
        _img = _titleImg.GetComponent<Image>();
    }

    private void Start() {
        CameraManager.Instance.StartCamera = _mainMenuCamera;

    }

    public void PlayGame() {
        CloseMenu();
    }

    public void QuitGame() {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void OpenMenu() {
        killTweens();
        CameraManager.Instance.SwitchCamera(_mainMenuCamera);
        gameObject.SetActive(true);
        _canvasGroup.DOFade(1, 2f);
        _playBtnRect.DOAnchorPosY(330, 0.25f).OnComplete(() => {
            _quitBtnRect.DOAnchorPosY(180, .25f).OnComplete(() => {
                _titleImgRect.DOAnchorPosY(-400, 0.5f);
                _img.DOFade(1, 0.5f);
            });
        });
    }

    public void CloseMenu() {
        killTweens();
        CameraManager.Instance.SwitchCamera(CameraManager.Instance.PlayerTopDownCamera);
        _quitBtnRect.DOAnchorPosY(-150, 0.25f).OnComplete(() => {
            _playBtnRect.DOAnchorPosY(0, 0.25f);
        });
        _titleImgRect.DOAnchorPosY(0, 1f);
        _img.DOFade(0, 1f);
        _canvasGroup.DOFade(0, 2f).OnComplete(() => gameObject.SetActive(false));
    }

    private void killTweens() {
        _titleImgRect.DOKill();
        _playBtnRect.DOKill();
        _quitBtnRect.DOKill();
        _img.DOKill();
        _canvasGroup.DOKill();
    }
}
