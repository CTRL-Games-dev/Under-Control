using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    RectTransform _rectTransform;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Update() {
        _rectTransform.Rotate(Vector3.up, -100 * Time.deltaTime);
    }
}
