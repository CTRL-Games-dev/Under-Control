using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class EvoUI : MonoBehaviour
{
    
    [SerializeField] private Image _bgImage;
    [SerializeField] private Image _lineImage;
    [SerializeField] private EvoUI _nextEvoUI;

    [SerializeField] private string _title;
    [SerializeField] private string _desc;
    [SerializeField] private string _flavor;
    [SerializeField] Tiers _tier;
    [SerializeField] private bool _isAvailable = false;
    [SerializeField] private bool _isSelected = false;

    [SerializeField] private List<Modifier> _modifiers = new List<Modifier>();
    [SerializeField] private ElementalType _elementalType;
    public ElementalType ElementalType => _elementalType;

    private RectTransform _rectTransform;
    private EventTrigger _eventTrigger;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _eventTrigger = GetComponent<EventTrigger>();

        _rectTransform.localScale = _isAvailable ? Vector3.one : Vector3.one * 0.95f;
        _bgImage.color = _isAvailable ? Color.white : Color.gray;
        _lineImage.fillAmount = _isSelected ? 1 : 0;
        _lineImage.color = ElementalInfo.GetColor(ElementalType);
    }


    public void SetAvailable() {
        _isAvailable = true;
        _eventTrigger.enabled = true;
        _bgImage.DOColor(Color.white, 0.2f * Settings.AnimationSpeed);
        _rectTransform.DOScale(1f, 0.2f * Settings.AnimationSpeed);
    }

    public void OnPointerEnter() {
        Player.UICanvas.InventoryCanvas.EvoInfo.SetInfo(_title, _desc, _flavor, _tier, ElementalType);
        if (!_isAvailable) return;
        _rectTransform.DOKill();
        _rectTransform.DOScale(1.1f, 0.2f * Settings.AnimationSpeed);
    }

    public void OnPointerExit() {
        if (!_isAvailable) return;
        _rectTransform.DOKill();
        _rectTransform.DOScale(1f, 0.2f * Settings.AnimationSpeed);
    }

    public void OnPointerClick() {
        if (!_isAvailable || _isSelected || Player.Instance.EvolutionPoints <= 0) return;
        Player.Instance.EvolutionPoints--;
        AddEvolution();
    }
    public void AddEvolution(){
        Player.Instance.SelectedEvolutions.Add(this);
        Player.Instance.OnEvolutionSelected.Invoke(this);
        _isSelected = true;
        if (_nextEvoUI != null) _nextEvoUI.SetAvailable();

         float fillAmount = 0;
        DOTween.To(() => fillAmount, x => fillAmount = x, 1, 0.8f * Settings.AnimationSpeed).SetEase(Ease.OutSine).OnUpdate(() => {
            _lineImage.fillAmount = fillAmount;
        }).OnComplete(() => {
            _rectTransform.DOScale(1.05f, 0.2f * Settings.AnimationSpeed);
        });
        _rectTransform.DOKill();
    }
    public List<Modifier> GetModifiers() {
        return new List<Modifier>(_modifiers);
    }


}
