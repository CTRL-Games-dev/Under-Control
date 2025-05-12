using UnityEngine;

public class IceAttack : PlayerWeaponAttack {
    public Effect IceEffect;
    public bool UseEffectDuration = true;

    public override void Attack(LivingEntity victim) {
        if (victim.CompareTag("Player")) return;

        victim.ApplyEffect(IceEffect);
        victim.TintAnimator.ApplyTint(TintColor, TintAlpha, UseEffectDuration ? IceEffect.Duration : Duration);
    }
}
