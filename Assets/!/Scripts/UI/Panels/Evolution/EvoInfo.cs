using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;



public class EvoInfo : MonoBehaviour
{
    [SerializeField] private TextLocalizer _titleTextLocalizer, _descText;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Image _outlineImg, _barImg;
    [SerializeField] private RectTransform _barRect;
    private ElementalType _currentEvoType;

    public void SetInfo(string title, string desc, ElementalType elementalType) {
        _titleTextLocalizer.Key = title;
        _descText.Key = desc;

        _titleText.DOKill();
        _outlineImg.DOKill();
        _barRect.DOKill();
        
        _titleText.DOColor(ElementalInfo.GetColor(elementalType), 0.6f * Settings.AnimationSpeed).SetEase(Ease.InOutCirc);
        _outlineImg.DOColor(ElementalInfo.GetColor(elementalType), 0.6f * Settings.AnimationSpeed).SetEase(Ease.InOutCirc);
        _barRect.DOScaleY(0, 0.3f * Settings.AnimationSpeed).SetEase(Ease.InOutCirc).OnComplete(() => {
            _barImg.sprite = ElementalInfo.GetBarSprite(elementalType);
            _barRect.DOScaleY(1, 0.3f * Settings.AnimationSpeed);
        });

        _currentEvoType = elementalType;
    }

}
