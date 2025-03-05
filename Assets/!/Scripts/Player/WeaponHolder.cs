using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public static Weapon UnknownWeaponPrefab => GameManager.Instance.UnknownWeaponPrefab;

    public PlayerController Player;
    private Weapon _currentWeapon;

    public void UpdateWeapon(WeaponItemData weaponData) {
        if (_currentWeapon != null) {
            Destroy(_currentWeapon.gameObject);
            _currentWeapon = null;
        }

        if (weaponData.Model != null) {
            if(!TryInstantiateWeapon(weaponData, out _currentWeapon)) {
                InstantiateUnknownWeapon(out _currentWeapon);
            }

            _currentWeapon.OnHit.AddListener(Player.OnWeaponHit);

            _currentWeapon.transform.localPosition = new Vector3(0, 0, 0);
            _currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }

    private bool TryInstantiateWeapon(WeaponItemData weaponData, out Weapon weapon) {
        weapon = null;

        if (weaponData.WeaponPrefab == null) {
            return false;
        }

        weapon = Instantiate(weaponData.WeaponPrefab, Vector3.zero, Quaternion.identity, transform);
        
        return true;
    }

    private void InstantiateUnknownWeapon(out Weapon weapon) {
        weapon = Instantiate(UnknownWeaponPrefab, Vector3.zero, Quaternion.identity, transform);
    }
}
