using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour {
    [HideInInspector]
    public UnityEvent<LivingEntity> OnHit = new();

    [SerializeField]
    private Collider Hitbox;

    [SerializeField] private ParticleSystem _majorTrail;
    [SerializeField] private GameObject _majorTrailObject;
    [SerializeField] private ParticleSystem _minorTrail;
    [SerializeField] private GameObject _minorTrailObject;

    void Start() {
        DisableHitbox();
    }

    public void OnTriggerEnter(Collider other) {
        if(!other.TryGetComponent(out LivingEntity victim)) return;

        OnHit?.Invoke(victim);
    }

    public void StartMajorTrail() {
        _majorTrailObject.SetActive(true);
        _majorTrail.Clear();
        _majorTrail.Play();
    }

    public void StartMinorTrail() {
        _minorTrailObject.SetActive(true);
        _minorTrail.Clear();
        _minorTrail.Play();
    }

    public void StopMinorTrail() {
        _minorTrail.Stop();
        _minorTrailObject.SetActive(false);
    }

    public void StopMajorTrail() {
        _majorTrail.Stop();
        _majorTrailObject.SetActive(false);
    }

    public void EnableHitbox() {
        Hitbox.enabled = true;
    }

    public void DisableHitbox() {
        Hitbox.enabled = false;
    }
}