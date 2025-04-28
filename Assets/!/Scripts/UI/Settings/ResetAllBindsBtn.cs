using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResetAllBindsBtn : MonoBehaviour
{
    [SerializeField] private List<Button> _resetBindBtns = new List<Button>();
    [SerializeField] private float _scaleSpeed = 0.4f;
    private RectTransform _rectTransform;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void ResetAllBinds()
    {
        _rectTransform.DOKill();
        _rectTransform.DOScale(new Vector3(0.9f, 0.9f, 1), (_scaleSpeed / 4) * Settings.AnimationSpeed)
            .SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => {
                _rectTransform.DOScale(new Vector3(1, 1, 1), (_scaleSpeed / 4) * Settings.AnimationSpeed)
                    .SetEase(Ease.OutBack).SetDelay((_scaleSpeed / 2) * Settings.AnimationSpeed).SetUpdate(true);
            });

        for (int i = 0; i < _resetBindBtns.Count; i++) {
            var resetBindBtn = _resetBindBtns[i];

            resetBindBtn.DOKill();
            resetBindBtn.transform.DOScale(Vector3.one, (0.05f * i) * Settings.AnimationSpeed)
                .SetUpdate(true).OnComplete(() => {
                    resetBindBtn.onClick.Invoke();
                });
        }
    }
}
