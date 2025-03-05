using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<LivingEntity> OnHit = new();

    public void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent(out LivingEntity victim)) return;

        OnHit?.Invoke(victim);
    }
}