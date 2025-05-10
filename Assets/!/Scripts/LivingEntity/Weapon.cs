using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour {
    [HideInInspector]
    public UnityEvent<LivingEntity> OnHit = new();

    [SerializeField]
    private Collider Hitbox;

    public WeaponTrait WeaponTrait = WeaponTrait.Basic;

    public void OnTriggerEnter(Collider other) {
        LivingEntity victim = other.GetComponentInParent<LivingEntity>(includeInactive: true);

        // Debug.Log($"Hitbox hit {victim} by {GetComponentInParent<LivingEntity>()}");

        if(victim == null) return;

        OnHit?.Invoke(victim);
    }

    public void EnableHitbox() {
        if(Hitbox == null) return;
        Hitbox.enabled = true;
    }

    public void DisableHitbox() {
        if(Hitbox == null) return;
        Hitbox.enabled = false;
    }
}