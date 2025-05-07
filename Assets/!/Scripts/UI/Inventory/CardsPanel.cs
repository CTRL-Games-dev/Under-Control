using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardsPanel : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Transform _cardsParent;
    [SerializeField] private TextLocalizer _nameTextLocalizer, _shortDescTextLocalizer, _longDescTextLocalizer;
    [SerializeField] private Image _barImg, _outlineImg, _icon;
    [SerializeField] private CanvasGroup _moreInfoCanvasGroup;
    [SerializeField] private RectTransform _barRect;

    void Start() {
        EventBus.RunCardClickedEvent.AddListener(OnCardClicked);
    }
    
    private void OnCardClicked(Card card) {
        AddCard(card);
    }
    public void AddCard(Card card) {
        GameObject cardGameobject = Instantiate(_cardPrefab, _cardsParent);
        CardUI cardUI = cardGameobject.GetComponent<CardUI>();
        cardUI.SetCard(card);
        cardUI.Setup();
        cardUI.IsInCollection = true;
    }

    public void ShowMoreInfo(Card card) {
        _moreInfoCanvasGroup.DOKill();
        if (card == null) {
            _moreInfoCanvasGroup.DOFade(0, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
            return;
        }

        _moreInfoCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        _nameTextLocalizer.Key = card.DisplayName;
        _shortDescTextLocalizer.Key = card.ShortDesc;
        _longDescTextLocalizer.Key = card.LongDesc;

        _icon.sprite = card.Icon == null ? ElementalInfo.GetIconSprite(card.ElementalType) : card.Icon;
        
        _barRect.DOScaleY(0, 0.3f * Settings.AnimationSpeed).SetEase(Ease.OutCubic).OnComplete(() => {
            _barImg.sprite = ElementalInfo.GetBarSprite(card.ElementalType);
            _barRect.DOScaleY(1, 0.3f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
        });

        _outlineImg.DOColor(ElementalInfo.GetColor(card.ElementalType), 0.6f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);
    }

}
