using UnityEngine;

public class LifeStealAttack : PlayerWeaponAttack {
    public float HealAmount;

    public override void Attack(LivingEntity victim) {
        if (victim.CompareTag("Player")) return;
        
        Player.LivingEntity.Health = Mathf.Clamp(Player.LivingEntity.Health + HealAmount, 0, Player.LivingEntity.MaxHealth);
    }
}
