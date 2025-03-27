using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class EvoUI : MonoBehaviour
{
    
    [SerializeField] private Image _bgImage;
    [SerializeField] private Image _lineImage;
    [SerializeField] private List<EvoUI> _nextEvoUIs;

    [SerializeField] private string _title;
    [SerializeField] private string _description;

    [SerializeField] private bool _isAvailable = false;
    [SerializeField] private bool _isSelected = false;

    private RectTransform _rectTransform;
    private EventTrigger _eventTrigger;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _eventTrigger = GetComponent<EventTrigger>();

        _rectTransform.localScale = _isAvailable ? Vector3.one : Vector3.one * 0.9f;
        _bgImage.color = _isAvailable ? Color.white : Color.gray;
        _lineImage.fillAmount = _isSelected ? 1 : 0;
    }


    public void SetAvailable() {
        _isAvailable = true;
        _eventTrigger.enabled = true;
        _bgImage.DOColor(Color.white, 0.2f);
        _rectTransform.DOScale(1f, 0.2f);
    }



    public void OnPointerEnter() {
        Player.UICanvas.EvoInfo.SetInfo(_title, _description, transform.position);
        if (!_isAvailable) return;
        _rectTransform.DOKill();
        _rectTransform.DOScale(1.1f, 0.2f);
    }

    public void OnPointerExit() {
        Player.UICanvas.EvoInfo.SetInfo(null, null, transform.position);
        if (!_isAvailable) return;
        _rectTransform.DOKill();
        _rectTransform.DOScale(1f, 0.2f);
    }

    public void OnPointerClick() {
        if (!_isAvailable || _isSelected) return;
        float fillAmount = 0;
        DOTween.To(() => fillAmount, x => fillAmount = x, 1, 0.5f).SetEase(Ease.InOutSine).OnUpdate(() => {
            _lineImage.fillAmount = fillAmount;
        }).OnComplete(() => {
            _bgImage.DOColor(Color.green, 0.2f);
        });
        _isSelected = true;
        _rectTransform.DOKill();
        foreach (EvoUI evoUI in _nextEvoUIs) {
            evoUI.SetAvailable();
        }
    }
}
