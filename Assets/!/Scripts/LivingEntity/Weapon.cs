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

    public void OnTriggerEnter(Collider other) {
        LivingEntity victim = other.GetComponentInParent<LivingEntity>(includeInactive: true);

        // Debug.Log($"Hitbox hit {victim} by {GetComponentInParent<LivingEntity>()}");

        if(victim == null) return;

        OnHit?.Invoke(victim);
    }

    public void StartMajorTrail() {
        if (_majorTrail == null) return;
        _majorTrailObject.SetActive(true);
        _majorTrail.Clear();
        _majorTrail.Play();
    }

    public void StartMinorTrail() {
        if (_majorTrail == null) return;
        _minorTrailObject.SetActive(true);
        _minorTrail.Clear();
        _minorTrail.Play();
    }

    public void StopMinorTrail() {
        if (_majorTrail == null) return;
        _minorTrail.Stop();
        _minorTrailObject.SetActive(false);
    }

    public void StopMajorTrail() {
        if (_majorTrail == null) return;
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