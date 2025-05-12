using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private struct ShakeData {
        public float Intensity;
        public float Duration;
        public float StartTime;
    }

    private static CinemachineCamera _currentCamera;
    private static CinemachineCamera _previousCamera = null;

    public static CameraManager Instance;

    private static CinemachineBasicMultiChannelPerlin _noise;

    private static List<ShakeData> _shakeData = new List<ShakeData>();

    private void Awake() {
        DontDestroyOnLoad(this);
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() {
        _noise = Player.Instance.CameraNoise;
    }

    void Update() {
        List<ShakeData> copyShakeDatas = new List<ShakeData>(_shakeData);

        float currentGain = 0;

        foreach(var shakeData in copyShakeDatas) {
            float progress = (Time.time - shakeData.StartTime) / shakeData.Duration;
            if(progress > 1) {
                _shakeData.Remove(shakeData);
                continue;
            }
        
            currentGain += shakeData.Intensity * (1 - progress);
        }

        _noise.AmplitudeGain = currentGain;
    }

    private static void shake(float intensity, float time) {
        _shakeData.Add(new ShakeData {
            Intensity = intensity,
            Duration = time,
            StartTime = Time.time
        });
    }

    public static void SwitchCamera(CinemachineCamera camera) {
        if (camera == _currentCamera) {
            return;
        }

        if(camera == null) {
            if (_previousCamera != null) {
                SwitchCamera(_previousCamera);
                return;
            } else {
                return;
            }
        }

        _previousCamera = _currentCamera == null ? camera : _currentCamera;

        _currentCamera = camera;

        setCamerasPriority();
    }

    public static bool IsCameraActive(CinemachineCamera camera) {
        return _currentCamera == camera;
    }

    public static void ShakeCamera(float strength, float time) {
        if (_currentCamera != null) {
            shake(strength, time);
        }
    }

    public static CinemachineCamera GetCurrentCamera() {
        return _currentCamera;
    }


    private static void setCamerasPriority() {
        List<CinemachineCamera> cameras = new List<CinemachineCamera>(FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None));

        for (int i = 0; i < cameras.Count; i++) {
            cameras[i].Priority = (cameras[i] == _currentCamera) ? 20 : 10;
        }
    }
}
