using UnityEngine;
using DG.Tweening;

public class ButtonRotator : MonoBehaviour
{
    private RectTransform _rectTransform;

    [SerializeField] private float _rotationSpeed = 0.4f; 

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void RotateButton() {
        _rectTransform.DOKill();
        _rectTransform.DORotate(new Vector3(0, 0, -360), _rotationSpeed, RotateMode.FastBeyond360).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => {
            _rectTransform.DORotate(new Vector3(0, 0, 0), 0).SetUpdate(true);
        });
        _rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1), _rotationSpeed / 4).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => {
            _rectTransform.DOScale(new Vector3(1, 1, 1), _rotationSpeed / 4).SetEase(Ease.OutBack).SetDelay(_rotationSpeed / 2).SetUpdate(true);
        });
    }
}
