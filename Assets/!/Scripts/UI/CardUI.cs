using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using NUnit.Framework;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TextLocalizer _nameTextLocalizer, _descriptionTextLocalizer;
    [SerializeField] private Image _barImg, _outlineImg, _icon;

    private Card _card;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private bool _isHovered = false;
    public bool IsInCollection = false;


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

    private void FixedUpdate() {
        if (_isHovered) {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 cardPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, _rectTransform.position);
            Vector2 direction = (mousePosition - cardPosition).normalized;

            float tiltAmount = 30f;
            Quaternion targetRotation = Quaternion.Euler(direction.y * tiltAmount, -direction.x * tiltAmount, 0);
            _rectTransform.rotation = Quaternion.Lerp(_rectTransform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }


    public void Setup() {
        gameObject.SetActive(true);
        Awake();

        _nameTextLocalizer.Key = _card.DisplayName;


        _descriptionTextLocalizer.Key = _card.ShortDesc;

        _icon.sprite = _card.Icon == null ? ElementalInfo.GetIconSprite(_card.ElementalType) : _card.Icon;        
        _barImg.sprite = ElementalInfo.GetBarSprite(_card.ElementalType);
        _outlineImg.color = ElementalInfo.GetColor(_card.ElementalType);

        _rectTransform.DOScale(Vector3.one, 0.5f * Settings.AnimationSpeed);
        _canvasGroup.DOFade(1, 0.3f * Settings.AnimationSpeed);
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
        _isHovered = true;
        _rectTransform.DOScale(Vector3.one * 1.1f, 0.3f * Settings.AnimationSpeed);
        if (IsInCollection) {
            Player.UICanvas.InventoryCanvas.CardsPanel.ShowMoreInfo(_card);
        } else {
            Player.UICanvas.ChooseCanvas.ShowLongdesc(_card.LongDesc);
        }
    }

    public void OnPointerExit() {
        _isHovered = false;
        _rectTransform.DORotate(Vector3.zero, 0.1f * Settings.AnimationSpeed);
        _rectTransform.DOScale(Vector3.one, 0.3f * Settings.AnimationSpeed);
        if (IsInCollection) {
            Player.UICanvas.InventoryCanvas.CardsPanel.ShowMoreInfo(null);
        } else {
            Player.UICanvas.ChooseCanvas.ShowLongdesc(null);
        }
    }

    
}
