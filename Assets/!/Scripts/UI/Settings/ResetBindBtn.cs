using UnityEngine;
using DG.Tweening;

public class ResetBindBtn : MonoBehaviour
{
    private RectTransform _rectTransform;
    [SerializeField] private RectTransform _targetRectTransform;

    [SerializeField] private float _rotationSpeed = 0.4f; 

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void RotateButton() {
        _rectTransform.DOKill();
        _targetRectTransform.DOKill();
        _rectTransform.DORotate(new Vector3(0, 0, -360), _rotationSpeed * Settings.AnimationSpeed, RotateMode.FastBeyond360)
            .SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => {
                _rectTransform.DORotate(new Vector3(0, 0, 0), 0).SetUpdate(true);
            });
        _targetRectTransform.DOScale(new Vector3(0.9f, 0.9f, 1), (_rotationSpeed / 4) * Settings.AnimationSpeed)
            .SetEase(Ease.OutBack).SetDelay((_rotationSpeed / 8) * Settings.AnimationSpeed).SetUpdate(true).OnComplete(() => {
                _targetRectTransform.DOScale(new Vector3(1, 1, 1), (_rotationSpeed / 4) * Settings.AnimationSpeed)
                    .SetEase(Ease.OutBack).SetDelay((_rotationSpeed / 4) * Settings.AnimationSpeed).SetUpdate(true);
            });
        _rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1), (_rotationSpeed / 4) * Settings.AnimationSpeed)
            .SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => {
                _rectTransform.DOScale(new Vector3(1, 1, 1), (_rotationSpeed / 4) * Settings.AnimationSpeed)
                    .SetEase(Ease.OutBack).SetDelay((_rotationSpeed / 2) * Settings.AnimationSpeed).SetUpdate(true);
            });
    }
}
