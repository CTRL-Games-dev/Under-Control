using Unity.VisualScripting;
using UnityEngine;

public class CharmAttack : PlayerWeaponAttack {
    protected override void OnHit(LivingEntity victim) {
        if (Random.Range(0f, 1f) < ApplyChance) return;
        if (victim.CompareTag("Player")) return;
        if (victim.Guild == Player.LivingEntity.Guild) return;
        if (victim.GetComponent<Charm>() != null) return;

        victim.TintAnimator.ApplyTint(TintColor, TintAlpha, Duration);
        victim.AddComponent<Charm>().Initialize(Duration);
    }
}
