using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TalkingCanvas : MonoBehaviour, IUICanvasState
{
    [SerializeField] private RectTransform _topBar, _bottomBar;
    [SerializeField] private RawImage _mageImage;
    private CanvasGroup _canvasGroup, _topBarCanvasGroup, _bottomBarCanvasGroup;
    
    
    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        _topBarCanvasGroup = _topBar.GetComponent<CanvasGroup>();
        _bottomBarCanvasGroup = _bottomBar.GetComponent<CanvasGroup>();
    } 


    public void ShowUI() {
        gameObject.SetActive(true);

        _canvasGroup.DOFade(1, 0.25f).SetEase(Ease.InOutSine).OnComplete(() => {
            _topBar.DOScaleY(1, 1f).SetEase(Ease.InOutSine);
            _topBarCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.InOutSine);
            
            _bottomBar.DOScaleY(1, 1f).SetEase(Ease.InOutSine).OnComplete(() => {
                _mageImage.DOFade(1, 0.5f).SetEase(Ease.InOutSine);
            });
            _bottomBarCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.InOutSine);
        });
    }

    public void HideUI() {
        _canvasGroup.DOFade(0, 0.1f).SetEase(Ease.InOutSine).OnComplete(() => {
            gameObject.SetActive(false);
        });
        _mageImage.DOFade(0, 0.5f).SetEase(Ease.InOutSine);
        _topBar.DOScaleY(0, 0.5f).SetEase(Ease.InOutSine);
        _topBarCanvasGroup.DOFade(0, 1f).SetEase(Ease.InOutSine);
        _bottomBar.DOScaleY(0, 0.5f).SetEase(Ease.InOutSine);
        _bottomBarCanvasGroup.DOFade(0, 1f).SetEase(Ease.InOutSine);
    }
}

