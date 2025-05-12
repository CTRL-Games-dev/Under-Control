using UnityEngine;

[CreateAssetMenu(fileName = "SO_Itm_HealthPotion", menuName = "Items/HealthPotion")]
public class HealthPotionItemData : ConsumableItemData {
    public float HealAmount;

    public override bool Consume(LivingEntity consumer) {
        consumer.Health += HealAmount;
        if(consumer.Health > consumer.MaxHealth) {
            consumer.Health = consumer.MaxHealth;
        }
        return true;
    }
}