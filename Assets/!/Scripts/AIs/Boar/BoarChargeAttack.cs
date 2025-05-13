using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class BoarChargeAttack : MonoBehaviour, IAttackAddon {
    public Effect StunEffect;

    public void OnHit(LivingEntity victim) {
        victim.ApplyEffect(StunEffect);
        victim.OnStunned.Invoke(StunEffect.Duration);
    }
}