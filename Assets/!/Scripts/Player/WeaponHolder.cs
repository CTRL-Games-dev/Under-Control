using UnityEngine;
using UnityEngine.Events;

public class WeaponHolder : MonoBehaviour
{
    public BoxCollider Collider;

    private GameObject _currentModelInstance;

    public UnityEvent<LivingEntity> OnWeaponHit = new();

    void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent(out LivingEntity victim)) return;

        OnWeaponHit?.Invoke(victim);
    }

    public void UpdateWeapon(WeaponItemData weapon) {
        // Collider.size = new Vector3(weapon.Range, weapon.Range, weapon.Range);

        if (_currentModelInstance != null) {
            Destroy(_currentModelInstance);
            _currentModelInstance = null;
        }

        if (weapon.Model != null) {
            _currentModelInstance = Instantiate(weapon.Model, Vector3.zero, Quaternion.identity, transform);
            _currentModelInstance.transform.localPosition = new Vector3(0, 0, 0);
            _currentModelInstance.transform.localRotation = Quaternion.identity;
        }
    }

    public void EnableHit() {
        Collider.enabled = true;
    }

    public void DisableHit() {
        Collider.enabled = false;
    }
}
