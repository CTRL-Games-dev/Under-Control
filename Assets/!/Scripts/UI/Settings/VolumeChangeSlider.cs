using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeChangeSlider : MonoBehaviour {
    [SerializeField] private AudioMixerGroup _audioMixerGroup;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private string _volumeParameterName = "Volume";
    [SerializeField, HideInInspector] private float _originalVolume = 0f;

    private void Start() {
        _audioMixerGroup.audioMixer.GetFloat(_volumeParameterName, out _originalVolume);

        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value) {
        if (value < 0.001f) {
            _audioMixerGroup.audioMixer.SetFloat(_volumeParameterName, -120);
            _valueText.text = "0%";
            return;
        }

        float gain = Mathf.Log10(value) * 20 + _originalVolume;

        _audioMixerGroup.audioMixer.SetFloat(_volumeParameterName, gain);
        _valueText.text = $"{(int)(value*100)}%";

    }
}
