using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CinemachineCamera _currentCamera;
    private CinemachineCamera _previousCamera = null;

    public static CameraManager Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;
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

    private void setCamerasPriority() {
        List<CinemachineCamera> cameras = new List<CinemachineCamera>(FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None));

        for (int i = 0; i < cameras.Count; i++) {
            cameras[i].Priority = (cameras[i] == _currentCamera) ? 20 : 10;
        }
    }
}
