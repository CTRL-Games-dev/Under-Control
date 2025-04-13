using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimationSpeedChange : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _valueText;
    private float _maxValue;

    private void Start() {
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        _maxValue = _slider.maxValue;
    }


    private void OnSliderValueChanged(float value) {
        _valueText.text = "x" + getActualValue(value);
        Settings.AnimationSpeed = getActualValue(value) != 0 ? 1f / getActualValue(value) : 0;
    }

    private float getActualValue(float value) {
        switch (value) {
            case -1:
                return 0;
            case 0:
                return 0.5f;
            case 1:
                return 1f;
            case 2:
                return 1.25f;
            case 3:
                return 1.5f;
            case 4:
                return 1.75f;
            default:
                return Mathf.Pow(2, value - 4);
        }
    }
}
