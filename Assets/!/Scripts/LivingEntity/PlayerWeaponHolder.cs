using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHolder : MonoBehaviour {
    public static Weapon UnknownWeaponPrefab => GameManager.Instance.UnknownWeaponPrefab;

    public LivingEntity Self;
    public SlashManager SlashManager;
    public bool PreventSelfDamage = true;

    private Weapon _currentWeaponHitter;
    private WeaponItemData _currentWeaponData;
    private float _currentWeaponPowerScale;
    private List<LivingEntity> _hitEntities = new List<LivingEntity>();
    private AttackType? _currentAttackType;
    private bool _isAttacking = false;

    public void UpdateWeapon(InventoryItem<WeaponItemData> weaponItem) {
        if (_currentWeaponHitter != null) {
            Destroy(_currentWeaponHitter.gameObject);
            _currentWeaponHitter = null;
            _currentWeaponData = null;
        }

        if(weaponItem == null) return;

        WeaponItemData weaponData = weaponItem.ItemData;
        float powerScale = weaponItem.PowerScale;

        if(weaponData == null) return;

        _currentWeaponPowerScale = powerScale;

        if (weaponData.WeaponPrefab != null) {
            _currentWeaponHitter = InstantiateWeapon(weaponData);
        } else {
            _currentWeaponHitter = InstantiateUnknownWeapon();
        }

        _currentWeaponData = weaponData;

        if (_currentWeaponHitter.gameObject.layer == LayerMask.NameToLayer("Default")) {
            _currentWeaponHitter.gameObject.layer = gameObject.layer;
        }

        _currentWeaponHitter.OnHit.AddListener(OnHit);

        SlashManager.OnHit.RemoveAllListeners();
        SlashManager.OnHit.AddListener((victim) => {
            _currentWeaponHitter.OnHit?.Invoke(victim);
        });

        DisableHitbox();
    }

    public void UpdateWeapon(WeaponItemData weaponData, float powerScale = 1) {
        if (_currentWeaponHitter != null) {
            Destroy(_currentWeaponHitter.gameObject);
            _currentWeaponHitter = null;
            _currentWeaponData = null;
        }

        if(weaponData == null) return;

        _currentWeaponPowerScale = powerScale;

        if (weaponData.WeaponPrefab != null) {
            _currentWeaponHitter = InstantiateWeapon(weaponData);
        } else {
            _currentWeaponHitter = InstantiateUnknownWeapon();
        }

        _currentWeaponData = weaponData;

        if (_currentWeaponHitter.gameObject.layer == LayerMask.NameToLayer("Default")) {
            _currentWeaponHitter.gameObject.layer = gameObject.layer;
        }

        _currentWeaponHitter.OnHit.AddListener(OnHit);
    }

    private Weapon InstantiateWeapon(WeaponItemData weaponData) {
        return Instantiate(weaponData.WeaponPrefab, transform); // nie ma clearowania rotacji i pozycji bo gracz ma rozne gripy 
    }

    private Weapon InstantiateUnknownWeapon() {
        return Instantiate(UnknownWeaponPrefab, Vector3.zero, Quaternion.identity, transform);
    }

    public void InitializeAttack(AttackType attackType) {
        if(_currentWeaponData == null) {
            Debug.LogWarning($"Current weapon is null");
            return;
        }

        if(_currentWeaponHitter == null) {
            Debug.LogWarning($"Current weapon hitter is null");
            return;
        }

        if(_currentAttackType != null) {
            Debug.LogWarning($"Attack type is not null");
            return;
        }

        _currentAttackType = attackType;
    }

    public void BeginAttack() {
        if(_currentWeaponData == null) {
            Debug.LogWarning($"Current weapon is null");
            return;
        }

        if(_currentAttackType == null) {
            Debug.LogWarning($"Current attack type is null");
            return;
        }

        if(_currentWeaponHitter == null) return;
        if(_isAttacking) return;

        _isAttacking = true;

        _hitEntities.Clear();
    }

    public void EndAttack() {
        if(_currentWeaponData == null) {
            // Debug.LogWarning($"Current weapon is null");
            return;
        }
        
        if(_currentWeaponHitter == null) return;
        if(_currentAttackType == null) return;
        if(!_isAttacking) return;

        _isAttacking = false;

        _hitEntities.Clear();
        _currentAttackType = null;
    }

    public void EnableHitbox() {
        if(_currentWeaponData == null) {
            Debug.LogWarning($"Current weapon is null");
            return;
        }
        
        if(_currentWeaponHitter == null) return;

        _currentWeaponHitter.EnableHitbox();
    }

    public void DisableHitbox() {
        if(_currentWeaponData == null) {
            // Debug.LogWarning($"Current weapon is null");
            return;
        }

        if(_currentWeaponHitter == null) return;

        _currentWeaponHitter.DisableHitbox();
    }

    public void OnHit(LivingEntity victim) {
        if(_currentWeaponData == null) {
            Debug.LogError($"Current weapon is null, but hit was registered!");
            return;
        }

        if(PreventSelfDamage && victim == Self) return;
        if(!victim.Guild.IsHostileTowards(Self.Guild)) return;
        if(_currentAttackType == null) return;

        float damageMin = _currentAttackType.Value == AttackType.LIGHT ? _currentWeaponData.LightDamageMin : _currentWeaponData.HeavyDamageMin;
        float damageMax = _currentAttackType.Value == AttackType.LIGHT ? _currentWeaponData.LightDamageMax : _currentWeaponData.HeavyDamageMax;

        damageMin *= _currentWeaponPowerScale;
        damageMax *= _currentWeaponPowerScale;

        if(damageMax <= 0) {
            Debug.LogWarning($"{Self.DebugName}: DamageMax is zero or negative. Current weapon is {_currentWeaponData.DisplayName}. Attack type is {_currentAttackType}");
            return;
        }

        if(damageMin < 0) {
            Debug.LogWarning($"{Self.DebugName}: DamageMin is negative. Current weapon is {_currentWeaponData.DisplayName}. Attack type is {_currentAttackType}");
            return;
        }

        if(damageMax < damageMin) {
            Debug.LogWarning($"{Self.DebugName}: DamageMax ({damageMax}) is less than DamageMin ({damageMin}). Current weapon is {_currentWeaponData.DisplayName}. Attack type is {_currentAttackType}");
            return;
        }

        if(_hitEntities.Contains(victim)) {
            // Debug.LogWarning($"Tried hitting {target.DisplayName} with {CurrentWeapon.DisplayName} but it was already hit");
            return;
        }

        float damageValue = Random.Range(damageMin, damageMax);

        if (_currentAttackType == AttackType.LIGHT) {
            damageValue = Self.ModifierSystem.CalculateForStatType(StatType.LIGHT_ATTACK_DAMAGE, damageValue);
        } else {
            damageValue = Self.ModifierSystem.CalculateForStatType(StatType.HEAVY_ATTACK_DAMAGE, damageValue);
        }

        Self.Attack(new Damage{
            Type = _currentWeaponData.DamageType,
            Value = damageValue
        }, victim);

        _hitEntities.Add(victim);
    }
}
