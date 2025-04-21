using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum EvoType {
    Fire,
    Ice,
    General
}

public class EvoInfo : MonoBehaviour
{
    [SerializeField] private TextLocalizer _titleTextLocalizer, _descText;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Color _fireColor, _iceColor, _generalColor;
    [SerializeField] private Sprite _fireSprite, _iceSprite, _generalSprite;
    [SerializeField] private Image _outlineImg, _barImg;
    [SerializeField] private RectTransform _barRect;
    private EvoType _currentEvoType;

    public void SetInfo(string title, string desc, EvoType evoType) {
        _titleTextLocalizer.Key = title;
        _descText.Key = desc;

        _titleText.DOKill();
        _outlineImg.DOKill();
        _barRect.DOKill();
        
        _titleText.DOColor(getColor(evoType), 0.6f * Settings.AnimationSpeed).SetEase(Ease.InOutCirc);
        _outlineImg.DOColor(getColor(evoType), 0.6f * Settings.AnimationSpeed).SetEase(Ease.InOutCirc);
        _barRect.DOScaleX(0, 0.3f * Settings.AnimationSpeed).SetEase(Ease.InOutCirc).OnComplete(() => {
            _barImg.sprite = getSprite(evoType);
            _barRect.DOScaleX(1, 0.3f * Settings.AnimationSpeed);
        });

        _currentEvoType = evoType;
    }

    private Color getColor(EvoType evoType) {
        switch (evoType) {
            case EvoType.Fire:
                return _fireColor;
            case EvoType.Ice:
                return _iceColor;
            default:
                return _generalColor;
        }
    }

    private Sprite getSprite(EvoType evoType) {
        switch (evoType) {
            case EvoType.Fire:
                return _fireSprite;
            case EvoType.Ice:
                return _iceSprite;
            default:
                return _generalSprite;
        }
    }
}
