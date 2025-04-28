using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeChangeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _audioMixerGroup;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private string _volumeParameterName = "Volume";

    private void Start() {
        _audioMixerGroup.audioMixer.SetFloat(_volumeParameterName, 0f);
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value) {
        _audioMixerGroup.audioMixer.SetFloat(_volumeParameterName, value);
        _valueText.text = ((int)(value)).ToString() + "db";

    }
}
