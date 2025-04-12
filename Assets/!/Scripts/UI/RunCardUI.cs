using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class RunCardUI : MonoBehaviour
{
    [SerializeField] private TextLocalizer _nameTextLocalizer, _descriptionTextLocalizer;
    [SerializeField] private Image _icon;

    private Card _card;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private bool _isHovered = false;


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

        _nameTextLocalizer.Key = _card.ModifierName;
        _icon.sprite = _card.Icon;
        _descriptionTextLocalizer.Key = _card.ModifierDescription;
        
        _rectTransform.DOScale(Vector3.one, 0.5f);
        _canvasGroup.DOFade(1, 0.3f);
    }

    public void DestroyCard() {
        _rectTransform.DOScale(Vector3.zero, 0.3f);
        _rectTransform.DOShakeRotation(0.3f, 90, 10, 90);
        _canvasGroup.DOFade(0, 0.3f).OnComplete(() => Destroy(gameObject));
    }

    public void SetCard(Card Card) {
        _card = Card;
    }

    private void OnRunCardClicked(Card Card) {
        gameObject.GetComponent<EventTrigger>().enabled = false;        
        DestroyCard();
    }


    public void OnClick() {
        _rectTransform.DOScale(Vector3.one * 1.4f, 0.3f);
        EventBus.RunCardClickedEvent?.Invoke(_card);
    }

    public void OnPointerEnter() {
        _isHovered = true;
        _rectTransform.DOScale(Vector3.one * 1.1f, 0.3f);
    }

    public void OnPointerExit() {
        _isHovered = false;
        _rectTransform.DORotate(Vector3.zero, 0.1f);
        _rectTransform.DOScale(Vector3.one, 0.3f);
    }

    
}
