using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class LanguageChoose : MonoBehaviour
{
    [SerializeField] private CanvasGroup _languageChooseCanvasGroup;
    private bool _languageChoosen = false;
    public UnityEvent LanguageChooseEvent;

    public void OnBtnPointerEnter(RectTransform rect) {
        if (_languageChoosen) return;
        rect.DOKill();
        rect.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnBtnPointerExit(RectTransform rect) {
        if (_languageChoosen) return;
        rect.DOKill();
        rect.DOScale(1, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnBtnPointerClick(RectTransform rect) {
        if (_languageChoosen) return;
        _languageChoosen = true;
        rect.DOKill();
        rect.DOScale(1.15f, 0.1f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => {
            rect.DOScale(1, 0.1f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => {
                _languageChooseCanvasGroup.DOFade(0, 0.2f).SetUpdate(true).OnComplete(() => {
                    _languageChooseCanvasGroup.gameObject.SetActive(false);
                    LanguageChooseEvent?.Invoke();
                });
            });
        });
    }

    public void ChangeLanguage(int i) {
        if (_languageChoosen) return;
        TextData.ChangeLanguage((Language)i);
    }
}
