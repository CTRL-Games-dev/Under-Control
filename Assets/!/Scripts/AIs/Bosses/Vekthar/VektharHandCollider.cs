using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class VektharHandCollider : MonoBehaviour {
    [HideInInspector] public Collider Collider;
    [HideInInspector] public UnityEvent<LivingEntity> TargetHit;
    private void Awake() {
        Collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) {
        LivingEntity victim = other.GetComponent<LivingEntity>();
        if(victim == null) return;

        TargetHit.Invoke(victim);

        Collider.enabled = false;
    }
}