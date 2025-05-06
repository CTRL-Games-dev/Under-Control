using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDVD : MonoBehaviour
{
    [SerializeField] private float _speed = 100f, _rotationSpeed = 100f;
    [SerializeField] private RectTransform _playerImgRect;
    [SerializeField] private RawImage _playerImg;
    [SerializeField] private List<Color> _colors = new List<Color>();
 
    private RectTransform _rectTransform;
    private Vector2 _direction = new Vector2(0.5f, 0.5f);


    void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Update() {
        _rectTransform.Translate(_direction * Time.deltaTime * _speed);
        if (_rectTransform.anchoredPosition.x > Screen.width / 2 - _rectTransform.sizeDelta.x / 2 || _rectTransform.anchoredPosition.x < -Screen.width / 2 + _rectTransform.sizeDelta.x / 2) {
            _direction.x *= -1;
            onWallHit();
        }
        if (_rectTransform.anchoredPosition.y > Screen.height / 2 - _rectTransform.sizeDelta.y / 2 || _rectTransform.anchoredPosition.y < -Screen.height / 2 + _rectTransform.sizeDelta.y / 2) {
            _direction.y *= -1;
            onWallHit();
        }
        _playerImgRect.Rotate(new Vector3(0, 0, _speed * Time.deltaTime * _rotationSpeed));
    }

    private void onWallHit() {
        _playerImg.color = _colors[Random.Range(0, _colors.Count - 1)];
    }
}
