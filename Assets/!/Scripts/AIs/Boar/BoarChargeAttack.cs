using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class BoarChargeAttack : MonoBehaviour {
    public Effect StunEffect;

    private List<LivingEntity> _alreadyHit = new List<LivingEntity>();

    void Start() {
        Weapon weapon = GetComponent<Weapon>();

        weapon.OnHit.AddListener(OnHit);
    }

    private void OnHit(LivingEntity victim) {
        if (_alreadyHit.Contains(victim)) return;
        
        _alreadyHit.Add(victim);
        victim.ApplyEffect(StunEffect);
        victim.OnStunned.Invoke(StunEffect.Duration);
    }
}