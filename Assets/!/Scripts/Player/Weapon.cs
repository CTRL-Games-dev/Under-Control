using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private Collider Hitbox;

    [HideInInspector]
    public UnityEvent<LivingEntity> OnHit = new();

    private WeaponsVfxController _vfxController;

    void Start()
    {
        _vfxController = GetComponentInChildren<WeaponsVfxController>();
    }

    public void OnTriggerEnter(Collider other)
    {
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