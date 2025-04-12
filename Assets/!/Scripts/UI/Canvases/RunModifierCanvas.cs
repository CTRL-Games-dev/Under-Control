using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChooseCanvas : MonoBehaviour, IUICanvasState
{
    [SerializeField] private GameObject _cardsHolder;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private RunModifier[] _runModifiers;

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
        foreach (RunModifier runModifier in _runModifiers) {
            _currentCards.Add(AddCard(runModifier));
        }

        _canvasGroup.DOComplete();
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1, 0.5f).OnComplete(() => {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
    
            foreach (RunCardUI card in _currentCards) {
                card.Setup();
            }
        });

    }


    public void HideUI() {
        if (_canvasGroup == null) return;
        _canvasGroup.DOFade(0, 0.5f).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

    private void OnRunCardClicked(RunModifier runModifier) {
        Player.LivingEntity.ApplyIndefiniteModifier(runModifier.Modifier);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
        _currentCards.Clear();
    }

    public RunCardUI AddCard(RunModifier _runModifier) {
        RunCardUI card = Instantiate(_cardPrefab, _cardsHolder.transform).GetComponent<RunCardUI>();
        card.SetRunModifier(_runModifier);
        return card;
    }

}
