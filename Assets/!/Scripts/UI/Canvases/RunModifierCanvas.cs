using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChooseCanvas : MonoBehaviour, IUICanvasState
{
    [SerializeField] private GameObject _cardsHolder;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Card[] _cards;

    private CanvasGroup _canvasGroup;
    private List<RunCardUI> _currentCards = new();

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void Start() {
        EventBus.RunCardClickedEvent.AddListener(OnRunCardClicked);
    }

    public void ShowUI() {
        gameObject.SetActive(true);
        foreach (Card card in _cards) {
            _currentCards.Add(AddCard(card));
        }

        _canvasGroup.DOComplete();
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).OnComplete(() => {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
    
            foreach (RunCardUI card in _currentCards) {
                card.Setup();
            }
        });

    }


    public void HideUI() {
        if (_canvasGroup == null) return;
        _canvasGroup.DOFade(0, 0.5f * Settings.AnimationSpeed).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

    private void OnRunCardClicked(Card card) {
        Player.LivingEntity.ApplyIndefiniteModifier(card.Modifier);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
        _currentCards.Clear();
    }

    public RunCardUI AddCard(Card runCard) {
        RunCardUI card = Instantiate(_cardPrefab, _cardsHolder.transform).GetComponent<RunCardUI>();
        card.SetCard(runCard);
        return card;
    }

}
