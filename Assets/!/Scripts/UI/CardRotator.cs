using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CardRotator : MonoBehaviour
{
    [SerializeField] private GameObject _backCard;
    [SerializeField] private GameObject _frontCard;
    [SerializeField] private RectTransform _holderRect;
 
    private Image _backCardImage, _frontCardImage;
    [SerializeField] private float _rotationSpeed = 0.3f;

    private bool _isFront = true;

    private void Awake() {
        _backCardImage = _backCard.GetComponent<Image>();
        _frontCardImage = _frontCard.GetComponent<Image>();        
    }


    public void OnClick() {
        _isFront = !_isFront;
        _holderRect.DOComplete();
        _holderRect.DOKill();

        if (_isFront) {
            _holderRect.DORotate(new Vector3(0, 90, 0), _rotationSpeed).SetEase(Ease.InQuint).OnComplete(() => {
                _backCardImage.raycastTarget = false;
                _backCard.SetActive(false);
                _frontCard.SetActive(true);
                _frontCardImage.raycastTarget = true;
                _holderRect.DORotate(new Vector3(0, 0, 0), _rotationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
                    _holderRect.rotation = Quaternion.Euler(0, 0, 0);
                });
            });
        } else {
            _holderRect.DORotate(new Vector3(0, 90, 0), _rotationSpeed).SetEase(Ease.InQuint).OnComplete(() => {
                _frontCardImage.raycastTarget = false;
                _frontCard.SetActive(false);
                _backCard.SetActive(true);
                _backCardImage.raycastTarget = true;
                _holderRect.DORotate(new Vector3(0, 180, 0), _rotationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
                    _holderRect.rotation = Quaternion.Euler(0, 0, 0);
                });
            });
        }

    }
}
