using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class ChooseCanvas : MonoBehaviour, IUICanvasState
{
    [SerializeField] private GameObject _cardsHolder;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private CanvasGroup _longDescCanvasGroup;
    [SerializeField] private TextLocalizer _longDescTextLocalizer;
    [SerializeField] private ContentSizeFitter  _contentSizeFitter;
    [SerializeField] private int _maxCards = 3;

    private CanvasGroup _canvasGroup;
    private CardUI[] _currentCards;

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _currentCards = new CardUI[_maxCards];
    }

    private void Start() {
        EventBus.RunCardClickedEvent.AddListener(OnRunCardClicked);
    }

    public void ShowUI() {
        gameObject.SetActive(true);
        _longDescCanvasGroup.alpha = 0;

        _currentCards = new CardUI[_maxCards];

        StopCoroutine(nameof(setupCards));
        StartCoroutine(nameof(setupCards));
    }

    private IEnumerator setupCards() {
        Card[] randomCards = GameManager.Instance.GetCards(_maxCards);
        for(int i = 0; i < randomCards.Length; i++) {
            _currentCards[i] = AddCard(randomCards[i]);
        }

        int length = _currentCards.Length;

        if (length % 2 == 0) {
            int half = length / 2;

            int index = 0; 
            for (int l = half -1; l >= 0; l--) {
                _currentCards[l].SetPosition(new Vector3(-250 * index -130, -20 * Mathf.Pow(index, 2), 0));
                _currentCards[l].SetTilt(5 + 5 * index);
                index++;
            }
            index = 0;
            for (int r = half; r < length; r++) {
                _currentCards[r].SetPosition(new Vector3(250 * index +130, -20 * Mathf.Pow(index, 2), 0));
                _currentCards[r].SetTilt(-5 + (-5) * index);
                index++;
            }
        } else {
            int half = length / 2;

            int index = 1; 
            for (int l = half -1; l >= 0; l--) {
                _currentCards[l].SetPosition(new Vector3(-250 * index, -20 * Mathf.Pow(index, 2), 0));
                _currentCards[l].SetTilt(5 + 5 * index);
                index++;
            }
            index = 1;
            for (int r = half +1; r < length; r++) {
                _currentCards[r].SetPosition(new Vector3(250 * index, -20 * Mathf.Pow(index, 2), 0));
                _currentCards[r].SetTilt(-5 + -5 * index);
                index++;
            }

            _currentCards[half].SetPosition(Vector3.zero);
            _currentCards[half].SetTilt(0);
        }

        _canvasGroup.DOComplete();
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed);
        yield return new WaitForSeconds(0.5f * Settings.AnimationSpeed);
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    
        foreach (CardUI card in _currentCards) {
            card.Setup();
            yield return new WaitForSeconds(0.1f * Settings.AnimationSpeed);
        }

        yield return new WaitForSeconds(0.2f);

        foreach (CardUI card in _currentCards) {
            card.RotateCard();
            yield return new WaitForSeconds(0.25f);
        }

        yield return null;
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
        WeaponCard weaponCard = card as WeaponCard;
        if (weaponCard != null) {
            if(!Player.Inventory.AddItem(weaponCard.WeaponData, 1, ItemRandomizer.GetPowerScale())) {
                GameObject prefab = weaponCard.WeaponData.WeaponPrefab.gameObject;
                Instantiate(prefab, Player.Instance.transform.position, Quaternion.identity);
            }
        }

        GameManager.Instance.ChooseCard(card);

        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.NotVisible);
        _currentCards = null;
    }

    public void ResetCardUI(){
        if (_currentCards == null) return;
        for (int i = 0; i < _currentCards.Length; i++) {
            if (_currentCards[i] != null) {
                _currentCards[i].DestroyCard();
            }
        }
        _currentCards = null;
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
