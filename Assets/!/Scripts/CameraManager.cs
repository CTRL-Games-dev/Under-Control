using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private List<CinemachineCamera> _cinemachineCameras = new();
    public CinemachineCamera PlayerTopDownCamera;

    public CinemachineCamera StartCamera;
    private CinemachineCamera _currentCamera;

    public static CameraManager Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start() {
        _cinemachineCameras = new List<CinemachineCamera>(FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None));
        
        StartCamera = (StartCamera == null) ? PlayerTopDownCamera : StartCamera;
        _currentCamera = StartCamera;

        setCamerasPriority();
    }

    public void SwitchCamera(CinemachineCamera camera) {
        if (!_cinemachineCameras.Contains(camera)) _cinemachineCameras.Add(camera);
        _currentCamera = camera;

        setCamerasPriority();
    }

    private void setCamerasPriority() {
        if (_cinemachineCameras == null) return;

        CinemachineCamera[] cameras = _cinemachineCameras.ToArray();

        for (int i = 0; i < cameras.Length; i++) {
            cameras[i].Priority = (cameras[i] == _currentCamera) ? 20 : 10;
        }
    }
}
