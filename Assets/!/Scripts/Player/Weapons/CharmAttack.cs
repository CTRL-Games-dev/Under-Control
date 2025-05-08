using UnityEngine;

public class CharmAttack : PlayerWeaponAttack {
    private LivingEntity _victim;
    private Guild _previousGuild;

    protected override void OnHit(LivingEntity victim) {
        if (Random.Range(0f, 1f) > ApplyChance || victim.CompareTag("Player")) return;
        _victim = victim;
        _victim.TintAnimator.ApplyTint(TintColor, TintAlpha, Duration);
        _previousGuild = _victim.Guild;
        _victim.Guild = Player.LivingEntity.Guild;
        Invoke(nameof(resetGuild), Duration);
    }

    private void resetGuild() {
        if (_victim != null) {
            _victim.Guild = _previousGuild;
            _victim.TintAnimator.ResetTint();
        }
    }
}
