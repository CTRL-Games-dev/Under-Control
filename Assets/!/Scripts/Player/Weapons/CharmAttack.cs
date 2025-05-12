using Unity.VisualScripting;
using UnityEngine;

public class CharmAttack : PlayerWeaponAttack {
    public override void Attack(LivingEntity victim) {
        if (victim.CompareTag("Player")) return;
        if (victim.Guild == Player.LivingEntity.Guild) return;
        if (victim.GetComponent<Charm>() != null) return;
        if (victim.AvoidGuildChange) return;

        victim.TintAnimator.ApplyTint(TintColor, TintAlpha, Duration);
        victim.AddComponent<Charm>().Initialize(Duration);
    }
}
