using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CinemachineCamera _currentCamera;

    public static CameraManager Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;
    }


    public void SwitchCamera(CinemachineCamera camera) {
        if(camera == null) {
            throw new ArgumentNullException("Camera cannot be null");
        }

        _currentCamera = camera;
        _currentCamera.Priority = 30;

        setCamerasPriority();
    }

    private void setCamerasPriority() {
        List<CinemachineCamera> cameras = new List<CinemachineCamera>(FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None));

        for (int i = 0; i < cameras.Count; i++) {
            cameras[i].Priority = (cameras[i] == _currentCamera) ? 20 : 10;
        }
    }
}
