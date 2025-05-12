using UnityEngine;

public class FireAttack : PlayerWeaponAttack {
    public Effect FireEffect;
    public bool UseEffectDuration = true;

    public override void Attack(LivingEntity victim) {
        if (Random.Range(0f, 1f) > ApplyChance || victim.CompareTag("Player")) return;
        victim.ApplyEffect(FireEffect);
        victim.TintAnimator.ApplyTint(TintColor, TintAlpha, UseEffectDuration ? FireEffect.Duration : Duration);
    }
}
