using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private Collider Hitbox;

    [HideInInspector]
    public UnityEvent<LivingEntity> OnHit = new();

    public void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent(out LivingEntity victim)) return;

        OnHit?.Invoke(victim);
    }

    public void EnableHitbox() {
        Hitbox.enabled = true;
    }

    public void DisableHitbox() {
        Hitbox.enabled = false;
    }
}