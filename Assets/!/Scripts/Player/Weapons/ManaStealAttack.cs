using UnityEngine;

public class ManaStealAttack : PlayerWeaponAttack {
    public float ManaAmount;

    public override void Attack(LivingEntity victim) {
        if (Random.Range(0f, 1f) > ApplyChance || victim.CompareTag("Player")) return;
        
        Player.LivingEntity.Mana = Mathf.Clamp(Player.LivingEntity.Mana + ManaAmount, 0, Player.LivingEntity.MaxMana);
    }
}
