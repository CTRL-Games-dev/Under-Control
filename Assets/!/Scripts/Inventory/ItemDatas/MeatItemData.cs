using UnityEngine;

[CreateAssetMenu(fileName = "SO_Itm_MeatItem", menuName = "Items/MeatItemData")]
public class MeatItemData : ConsumableItemData {
    public float HealthRegen;

    public override bool Consume(LivingEntity consumer) {
        consumer.Health += HealthRegen;
        if (consumer.Health > consumer.MaxHealth) {
            consumer.Health = consumer.MaxHealth;
        }
        return true;
    }
}