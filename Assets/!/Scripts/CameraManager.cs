using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum CameraType {
        PlayerTopDown,
        MainMenu
    }

    [SerializeField] private List<CinemachineCamera> _cinemachineCameras;
    [SerializeField] private CinemachineCamera _playerTopDownCamera;
    [SerializeField] private CinemachineCamera _mainMenuCamera;

    public CinemachineCamera StartCamera;
    private CinemachineCamera _currentCamera;

    public static CameraManager Instance;

    void Start() {
        Instance = this;
        
        _currentCamera = StartCamera;

        foreach (var camera in _cinemachineCameras) {
            camera.Priority = camera == _currentCamera ? 20 : 10;
        }
    }

    public void SwitchCamera(CameraType type) {
        switch (type) {
            case CameraType.PlayerTopDown:
                _currentCamera = _playerTopDownCamera;
                break;
            case CameraType.MainMenu:
                _currentCamera = _mainMenuCamera;
                break;
        }

        foreach (var camera in _cinemachineCameras) {
            camera.Priority = camera == _currentCamera ? 20 : 10;
        }
    }

    public bool AddCamera(CinemachineCamera camera) {
        if (_cinemachineCameras.Contains(camera)) {
            return false;
        }

        _cinemachineCameras.Add(camera);
        return true;
    }
}
