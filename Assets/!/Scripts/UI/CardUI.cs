using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TextLocalizer _nameTextLocalizer, _descriptionTextLocalizer;
    [SerializeField] private Image _barImg, _outlineImg, _icon;
    [SerializeField] private GameObject _backCard, _frontCard;
    [SerializeField] private RectTransform _holderRect;

    private Card _card;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    public bool IsInCollection = false;
    private float _zTilt = 0f;


    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();

        _rectTransform.localScale = Vector3.zero;
        _canvasGroup.alpha = 0;
    }

    private void OnDestroy() {
        EventBus.RunCardClickedEvent.RemoveListener(OnRunCardClicked);
    }


    private void Start() {
        EventBus.RunCardClickedEvent.AddListener(OnRunCardClicked);
    }


    public void SetTilt(float tiltAmount) {
        _zTilt = tiltAmount;
        _rectTransform.localRotation = Quaternion.Euler(0, 0, tiltAmount);
    }

    public void SetPosition(Vector3 position) {
        _rectTransform.anchoredPosition = position;
    }


    public void Setup() {
        gameObject.SetActive(true);
        Awake();

        _rectTransform.DOScale(Vector3.one, 0.5f * Settings.AnimationSpeed);
        _canvasGroup.DOFade(1, 0.3f * Settings.AnimationSpeed);
    }

    public void RotateCard() {
        _rectTransform.DOComplete();
        _holderRect.DOKill();
        _holderRect.DOLocalRotate(new Vector3(0, 90, 0), 0.25f).SetEase(Ease.InCirc).OnComplete(() => {
            _backCard.SetActive(false);
            _frontCard.SetActive(true);
            
            _nameTextLocalizer.Key = _card.DisplayName;
            _descriptionTextLocalizer.Key = _card.ShortDesc;

            _icon.sprite = _card.Icon == null ? ElementalInfo.GetIconSprite(_card.ElementalType) : _card.Icon;        
            _barImg.sprite = ElementalInfo.GetBarSprite(_card.ElementalType);
            _outlineImg.color = ElementalInfo.GetColor(_card.ElementalType);
            
            _holderRect.DOLocalRotate(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.InCirc).OnComplete(() => {
                _holderRect.localRotation = Quaternion.Euler(0, 0, 0);
            });
        });
    }




    public void DestroyCard() {
        _rectTransform.DOScale(Vector3.zero, 0.3f * Settings.AnimationSpeed);
        _rectTransform.DOShakeRotation(0.3f * Settings.AnimationSpeed, 90, 10, 90);
        _canvasGroup.DOFade(0, 0.3f * Settings.AnimationSpeed).OnComplete(() => Destroy(gameObject));
    }

    public void SetCard(Card Card) {
        _card = Card;
    }

    private void OnRunCardClicked(Card Card) {
        if (IsInCollection) return;
        gameObject.GetComponent<EventTrigger>().enabled = false;
        DestroyCard();
    }


    public void OnClick() {
        if (IsInCollection) return;
        _rectTransform.DOScale(Vector3.one * 1.4f, 0.3f * Settings.AnimationSpeed);
        EventBus.RunCardClickedEvent?.Invoke(_card);
    }

    public void OnPointerEnter() {
        _rectTransform.DOScale(Vector3.one * 1.1f, 0.3f * Settings.AnimationSpeed);
        if (IsInCollection) {
            Player.UICanvas.InventoryCanvas.CardsPanel.ShowMoreInfo(_card);
        } else {
            Player.UICanvas.ChooseCanvas.ShowLongdesc(_card.LongDesc);
        }
    }

    public void OnPointerExit() {
        _rectTransform.DOScale(Vector3.one, 0.3f * Settings.AnimationSpeed);
        if (IsInCollection) {
            Player.UICanvas.InventoryCanvas.CardsPanel.ShowMoreInfo(null);
        } else {
            Player.UICanvas.ChooseCanvas.ShowLongdesc(null);
        }
    }

    
}
