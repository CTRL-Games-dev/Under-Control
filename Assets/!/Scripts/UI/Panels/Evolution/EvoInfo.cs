using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public enum Tiers{
    Tier1,
    Tier2,
    Tier3
}
public class EvoInfo : MonoBehaviour
{
    [SerializeField] private TextLocalizer _titleTextLocalizer, _descText, _flavorText;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Image _outlineImg, _barImg;
    [SerializeField] private RectTransform _barRect;
    [SerializeField] private Color _tier1Color = new(0.749f, 0.431f, 0);
    [SerializeField] private Color _tier2Color = new(0.82f, 0.82f, 0.82f);
    [SerializeField] private Color _tier3Color = new(1f, 0.941f, 0.243f);
    private ElementalType _currentEvoType;

    public void SetInfo(string title, string desc, string flavor, Tiers tier, ElementalType elementalType) {
        _titleTextLocalizer.Key = title;
        _descText.Key = desc;
        _flavorText.Key = flavor;

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
