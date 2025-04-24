using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ChooseCanvas : MonoBehaviour, IUICanvasState
{
    [SerializeField] private GameObject _cardsHolder;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Card[] _cards;
    [SerializeField] private CanvasGroup _longDescCanvasGroup;
    [SerializeField] private TextLocalizer _longDescTextLocalizer;
    [SerializeField] private ContentSizeFitter  _contentSizeFitter;

    private CanvasGroup _canvasGroup;
    private List<CardUI> _currentCards = new();

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
        _longDescCanvasGroup.alpha = 0;
        
        for (int i = 0; i < 3; i++) {
            Card card = _cards[Random.Range(0, _cards.Length)];
            _currentCards.Add(AddCard(card));
        }

        _canvasGroup.DOComplete();
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).OnComplete(() => {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
    
            foreach (CardUI card in _currentCards) {
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
        ModifierCard modifierCard = card as ModifierCard;
        if (modifierCard != null) {
            Player.LivingEntity.ApplyIndefiniteModifier(modifierCard.Modifier);
        }
        SpellCard spellCard = card as SpellCard;
        if (spellCard != null) {
            if (Player.Instance.SpellSlotOne == null) {
                Player.Instance.SpellSlotOne = spellCard.Spell;
                Player.UICanvas.HUDCanvas.SetSpellCooldownColor(1, ElementalInfo.GetColor(card.ElementalType));
            } else if (Player.Instance.SpellSlotTwo == null) {
                Player.Instance.SpellSlotTwo = spellCard.Spell;
                Player.UICanvas.HUDCanvas.SetSpellCooldownColor(2, ElementalInfo.GetColor(card.ElementalType));
            } else if (Player.Instance.SpellSlotThree == null) {
                Player.Instance.SpellSlotThree = spellCard.Spell;
                Player.UICanvas.HUDCanvas.SetSpellCooldownColor(3, ElementalInfo.GetColor(card.ElementalType));
            } else {
                Debug.Log("All slots are full!");
            }
        }

        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
        _currentCards.Clear();
    }

    public CardUI AddCard(Card runCard) {
        CardUI card = Instantiate(_cardPrefab, _cardsHolder.transform).GetComponent<CardUI>();
        card.SetCard(runCard);
        return card;
    }

    public void ShowLongdesc(string key) {
        _longDescCanvasGroup.DOKill();
        if (string.IsNullOrEmpty(key)) {
            _longDescCanvasGroup.DOFade(0, 0.5f * Settings.AnimationSpeed);
            return;
        }
        
        _contentSizeFitter.enabled = false;
        _longDescTextLocalizer.Key = key;
        _contentSizeFitter.enabled = true;
        _longDescCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed);
    }

}
