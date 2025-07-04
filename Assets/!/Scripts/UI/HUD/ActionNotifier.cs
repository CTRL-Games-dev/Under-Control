using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class ActionNotifier : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextLocalizer _nameTextLocalizer;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _amountText;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private float _goalY = 0;


    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(Sprite icon, string text, Color color, int amount, ActionNotifierManager parent)
    {
        _icon.sprite = icon;
        _nameTextLocalizer.Key = text;
        _nameText.color = color;
        _amountText.text = "×" + amount.ToString();

        _rectTransform.DOScale(1f, 0.3f * Settings.AnimationSpeed).SetEase(Ease.InExpo).OnComplete(() => {
            _rectTransform.DOAnchorPosX(_rectTransform.anchoredPosition.x + _rectTransform.sizeDelta.x, 0.5f * Settings.AnimationSpeed).SetDelay(3f * Settings.AnimationSpeed).SetEase(Ease.InSine);
            _canvasGroup.DOFade(0, 0.3f * Settings.AnimationSpeed).SetDelay(3.2f * Settings.AnimationSpeed).OnComplete(() => {
                parent.TryClearChildren(this);
            });
        });
    }

    public void MoveUp() {
        _rectTransform.DOAnchorPosY(_goalY, 0).OnComplete(() => {
            _goalY += 70;
            _rectTransform.DOAnchorPosY(_goalY, 0.2f * Settings.AnimationSpeed).SetEase(Ease.OutSine);
        });
    }

    public void Destroy() {
        Destroy(gameObject);   
    }
}
