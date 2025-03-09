using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour {
    [HideInInspector]
    public UnityEvent<LivingEntity> OnHit = new();

    [SerializeField]
    private Collider Hitbox;
    private WeaponVfxController _vfxController;

    void Start() {
        _vfxController = GetComponentInChildren<WeaponVfxController>();

        // Disable hitbox by default
        DisableHitbox();
    }

    public void OnTriggerEnter(Collider other) {
        if(!other.TryGetComponent(out LivingEntity victim)) return;

        OnHit?.Invoke(victim);
    }

    public void EnableHitbox() {
        Hitbox.enabled = true;
        _vfxController?.StartTrail();
    }

    public void DisableHitbox() {
        Hitbox.enabled = false;
        _vfxController?.StopTrail();
    }
}