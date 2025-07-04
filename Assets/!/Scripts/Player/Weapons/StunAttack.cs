using UnityEngine;

public class StunAttack : PlayerWeaponAttack {
    public Effect StunEffect;
    public bool UseEffectDuration = true;

    public override void Attack(LivingEntity victim) {
        if (victim.CompareTag("Player")) return;

        victim.ApplyEffect(StunEffect);
        victim.TintAnimator.ApplyTint(TintColor, TintAlpha, UseEffectDuration ? StunEffect.Duration : Duration);
    }
}
