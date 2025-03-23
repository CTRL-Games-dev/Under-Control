using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public static Weapon UnknownWeaponPrefab => GameManager.Instance.UnknownWeaponPrefab;

    public LivingEntity Self;
    public bool PreventSelfDamage = true;

    private Weapon _currentWeaponHitter;
    private WeaponItemData _currentWeaponData;
    private List<LivingEntity> _hitEntities = new List<LivingEntity>();

    public void UpdateWeapon(WeaponItemData weaponData) {
        if (_currentWeaponHitter != null) {
            Destroy(_currentWeaponHitter.gameObject);
            _currentWeaponHitter = null;
            _currentWeaponData = null;
        }

        if (weaponData.WeaponPrefab != null) {
            _currentWeaponHitter = InstantiateWeapon(weaponData);
        } else {
            _currentWeaponHitter = InstantiateUnknownWeapon();
        }

        _currentWeaponData = weaponData;

        _currentWeaponHitter.gameObject.layer = gameObject.layer;
        _currentWeaponHitter.OnHit.AddListener(OnHit);
    }

    private Weapon InstantiateWeapon(WeaponItemData weaponData) {
        return Instantiate(weaponData.WeaponPrefab, transform); // nie ma clearowania rotacji i pozycji bo gracz ma rozne gripy 
    }

    private Weapon InstantiateUnknownWeapon() {
        return Instantiate(UnknownWeaponPrefab, Vector3.zero, Quaternion.identity, transform);
    }

    public void BeginAttack() {
        if(_currentWeaponHitter == null) return;
        _hitEntities.Clear();
        _currentWeaponHitter.StartMinorTrail();
    }

    public void EndAttack() {
        if(_currentWeaponHitter == null) return;
        _hitEntities.Clear();
        _currentWeaponHitter.StopMinorTrail();
    }

    public void EnableHitbox() {
        if(_currentWeaponHitter == null) return;
        _currentWeaponHitter.EnableHitbox();
        _currentWeaponHitter.StartMajorTrail();
    }

    public void DisableHitbox() {
        if(_currentWeaponHitter == null) return;
        _currentWeaponHitter.DisableHitbox();
        _currentWeaponHitter.StopMajorTrail();
    }

    public void OnHit(LivingEntity victim) {
        if(PreventSelfDamage && victim == Self) return;
        if(!victim.Guild.IsHostileTowards(Self.Guild)) return;

        if(_currentWeaponData.DamageMax <= 0) {
            Debug.LogWarning($"{Self.DebugName}: DamageMax is zero or negative. Current weapon is {_currentWeaponData.DisplayName}");
            return;
        }

        if(_currentWeaponData.DamageMin < 0) {
            Debug.LogWarning($"{Self.DebugName}: DamageMin is negative. Current weapon is {_currentWeaponData.DisplayName}");
            return;
        }

        if(_currentWeaponData.DamageMax < _currentWeaponData.DamageMin) {
            Debug.LogWarning($"{Self.DebugName}: DamageMax ({_currentWeaponData.DamageMax}) is less than DamageMin ({_currentWeaponData.DamageMin}). Current weapon is {_currentWeaponData.DisplayName}");
            return;
        }

        if(_hitEntities.Contains(victim)) {
            // Debug.LogWarning($"Tried hitting {target.DisplayName} with {CurrentWeapon.DisplayName} but it was already hit");
            return;
        }

        float damageValue = Random.Range(_currentWeaponData.DamageMin, _currentWeaponData.DamageMax);

        victim.TakeDamage(new Damage{
            Type = _currentWeaponData.DamageType,
            Value = damageValue
        }, Self);

        _hitEntities.Add(victim);
    }
}
