using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class ActionNotifier : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _amountText;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;


    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(Sprite icon, string text, Color color, int amount, ActionNotifierManager parent)
    {
        _icon.sprite = icon;
        _nameText.text = text;
        _nameText.color = color;
        _amountText.text = "×" + amount.ToString();

        _rectTransform.DOScale(1f, 0.3f).SetEase(Ease.InExpo).OnComplete(() => {
            _rectTransform.DOAnchorPosX(_rectTransform.anchoredPosition.x + _rectTransform.sizeDelta.x, 0.5f).SetDelay(3f).SetEase(Ease.InSine);
            _canvasGroup.DOFade(0, 0.3f).SetDelay(3.2f).OnComplete(() => {
                parent.TryClearChildren(this);
            });
        });

    }

    public void Destroy() {
        Destroy(gameObject);   
    }
}
