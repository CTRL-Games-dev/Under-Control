using UnityEngine;

public class LifeStealAttack : PlayerWeaponAttack {
    public float HealAmount;

    protected override void OnHit(LivingEntity victim) {
        if (Random.Range(0f, 1f) > ApplyChance || victim.CompareTag("Player")) return;
        
        Player.LivingEntity.Health = Mathf.Clamp(Player.LivingEntity.Health + HealAmount, 0, Player.LivingEntity.MaxHealth);
    }
}
