using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private CinemachineCamera _mainMenuCamera;
    [SerializeField] private RectTransform _blackBarRect;    

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();   
    }

    private void Start() {
        CameraManager.Instance.StartCamera = _mainMenuCamera;

    }

    public void PlayGame() {
        UICanvas.Instance.CloseUIState(UIState.MainMenu);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void OpenMenu() {
        killTweens();
        CameraManager.Instance.SwitchCamera(_mainMenuCamera);
        gameObject.SetActive(true);
        _blackBarRect.DOAnchorPosY(0, 1).SetEase(Ease.InOutBack);
    }

    public void CloseMenu() {
        killTweens();
        CameraManager.Instance.SwitchCamera(CameraManager.Instance.PlayerTopDownCamera);
        _blackBarRect.DOAnchorPosY(720, 1).SetEase(Ease.InOutBack).OnComplete(() => gameObject.SetActive(false));
    }

    private void killTweens() {
        _blackBarRect.DOKill();
        _canvasGroup.DOKill();
    }
}
