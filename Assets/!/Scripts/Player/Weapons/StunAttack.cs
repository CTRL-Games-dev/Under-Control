using UnityEngine;

public class StunAttack : PlayerWeaponAttack {
    public Effect StunEffect;
    public bool UseEffectDuration = true;

    protected override void OnHit(LivingEntity victim) {
        if (Random.Range(0f, 1f) > ApplyChance || victim.CompareTag("Player")) return;
        victim.ApplyEffect(StunEffect);
        victim.TintAnimator.SetTint(TintColor, TintAlpha, UseEffectDuration ? StunEffect.Duration : Duration);
    }
}
