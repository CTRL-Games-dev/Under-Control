using Unity.Cinemachine;
using UnityEngine;


public class CameraShake : MonoBehaviour{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private CinemachineBasicMultiChannelPerlin noice;


    private float startingIntensity;
    private float shakeTimer;
    private float shakeTimerTotal;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    void Update() {
        if (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            noice.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - shakeTimer / shakeTimerTotal);
        }
    }

    public void Shake(float intensity, float time) {
        noice.AmplitudeGain = intensity;
        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }
}