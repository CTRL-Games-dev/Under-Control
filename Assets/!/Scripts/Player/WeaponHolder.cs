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

        if (weaponData.WeaponPrefab != null) {
            _currentWeapon = InstantiateWeapon(weaponData);
        } else {
            _currentWeapon = InstantiateUnknownWeapon();
        }

        _currentWeapon.OnHit.AddListener(Player.OnWeaponHit);

        _currentWeapon.transform.localPosition = new Vector3(0, 0, 0);
        _currentWeapon.transform.localRotation = Quaternion.identity;
    }

    private Weapon InstantiateWeapon(WeaponItemData weaponData) {
        return Instantiate(weaponData.WeaponPrefab, Vector3.zero, Quaternion.identity, transform);
    }

    private Weapon InstantiateUnknownWeapon() {
        return Instantiate(UnknownWeaponPrefab, Vector3.zero, Quaternion.identity, transform);
    }

    public void BeginAttack() {
        if(_currentWeapon == null) return;
        _currentWeapon.EnableHitbox();
    }

    public void EndAttack() {
        if(_currentWeapon == null) return;
        _currentWeapon.DisableHitbox();
    }
}
