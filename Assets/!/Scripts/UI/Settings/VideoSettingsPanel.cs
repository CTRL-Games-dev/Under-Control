using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class VideoSettingsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private Toggle _fullscreenToggle, _vsyncToggle;
    [SerializeField] private RectTransform _toggleRect, _vsyncRect;
    [SerializeField] private Slider _fpsSlider;
    [SerializeField] private TextLocalizer _fpsText;

    private List<Vector2Int> _uniqueResolutions;
    private bool _isFullscreen;
    private int _selectedResolutionIndex = -1;

    void Start() {
        _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        _fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
        _vsyncToggle.onValueChanged.AddListener(OnVsyncToggleChanged);
        _fpsSlider.onValueChanged.AddListener(OnFpsSliderChanged);

        Screen.fullScreenMode = FullScreenMode.Windowed;
        _isFullscreen = Screen.fullScreen;
        _fullscreenToggle.isOn = _isFullscreen;


        _uniqueResolutions = getUniqueResolutions(Screen.resolutions);

        List<string> resolutionStrings = new List<string>();

        foreach (var res in _uniqueResolutions) {
            resolutionStrings.Add($"{res.x} x {res.y}");
            if (res.x == Screen.currentResolution.width && res.y == Screen.currentResolution.height) {
                _selectedResolutionIndex = resolutionStrings.Count - 1;
            }
        }

        if (_selectedResolutionIndex == -1) {
            resolutionStrings.Add($"{Screen.currentResolution.width} x {Screen.currentResolution.height}");
            _uniqueResolutions.Add(new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height));
            _selectedResolutionIndex = _uniqueResolutions.Count - 1;
        }

        _resolutionDropdown.AddOptions(resolutionStrings);
        _resolutionDropdown.value = _selectedResolutionIndex;
    }

    private List<Vector2Int> getUniqueResolutions(Resolution[] resolutions) {
        List<Vector2Int> uniqueResolutions = new List<Vector2Int>();
        foreach (var res in resolutions) {
            if (res.width == 0 || res.height == 0) continue;
            Vector2Int resolution = new Vector2Int(res.width, res.height);
            if (!uniqueResolutions.Contains(resolution)) {
                uniqueResolutions.Add(resolution);
            }
        }
        return uniqueResolutions;
    }


    public void OnResolutionChanged(int index) {
        if (index >= _uniqueResolutions.Count) return;
        _selectedResolutionIndex = index;

        Vector2Int selectedResolution = _uniqueResolutions[index];

        Screen.SetResolution(selectedResolution.x, selectedResolution.y, _isFullscreen);
    }
    
    public void OnFullscreenToggleChanged(bool isOn) {
        _isFullscreen = isOn;
        Screen.fullScreen = _isFullscreen;
        if (_isFullscreen) {
            _toggleRect.DOScale(Vector3.one * 1.3f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutBack).OnComplete(() => {
                _toggleRect.DOScale(Vector3.one, 0.1f * Settings.AnimationSpeed);
            });
        }
    }

    public void OnVsyncToggleChanged(bool isOn) {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
        if (isOn) {
            _vsyncRect.DOScale(Vector3.one * 1.3f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutBack).OnComplete(() => {
                _vsyncRect.DOScale(Vector3.one, 0.1f * Settings.AnimationSpeed);
            });
        }
    }

    public void OnFpsSliderChanged(float value) {
        int fps = Mathf.RoundToInt(value);
        _vsyncToggle.isOn = false;
        QualitySettings.vSyncCount = 0;
        if (value == _fpsSlider.maxValue) {
            _fpsText.Key = "ui_unlimited_fps_key";
            Application.targetFrameRate = -1;
            return;
        }

        Application.targetFrameRate = fps * 5;
        _fpsText.Key = (fps * 5).ToString();
    }
}
