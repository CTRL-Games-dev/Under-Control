using UnityEngine;

public class IceAttack : PlayerWeaponAttack {
    public Effect IceEffect;
    public bool UseEffectDuration = true;

    protected override void OnHit(LivingEntity victim) {
        if (Random.Range(0f, 1f) > ApplyChance || victim.CompareTag("Player")) return;
        victim.ApplyEffect(IceEffect);
        victim.TintAnimator.ApplyTint(TintColor, TintAlpha, UseEffectDuration ? IceEffect.Duration : Duration);
    }
}
