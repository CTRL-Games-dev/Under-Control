using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CinemachineCamera _currentCamera;
    private CinemachineCamera _previousCamera = null;

    public static CameraManager Instance;


    private CinemachineBasicMultiChannelPerlin _noise;

    private float _startingIntensity;
    private float _shakeTimer;
    private float _shakeTimerTotal;


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
    if (_shakeTimer > 0) {
        _shakeTimer -= Time.deltaTime;
        _noise.AmplitudeGain = Mathf.Lerp(_startingIntensity, 0f, 1 - _shakeTimer / _shakeTimerTotal);
        }
    }

    private void shake(float intensity, float time) {
        _noise.AmplitudeGain = intensity;
        _startingIntensity = intensity;
        _shakeTimerTotal = time;
        _shakeTimer = time;
    }


    public void SwitchCamera(CinemachineCamera camera) {
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

    public void ShakeCamera(float strength, float time) {
        if (_currentCamera != null) {
            shake(strength, time);
        }
    }


    private void setCamerasPriority() {
        List<CinemachineCamera> cameras = new List<CinemachineCamera>(FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None));

        for (int i = 0; i < cameras.Count; i++) {
            cameras[i].Priority = (cameras[i] == _currentCamera) ? 20 : 10;
        }
    }
}
