using UnityEngine;

[CreateAssetMenu(fileName = "SO_Itm_ManaPotion", menuName = "Items/ManaPotion")]
public class ManaPotionItemData : ConsumableItemData {
    public float ManaGainAmount;    

    public override bool Consume(LivingEntity consumer) {
        consumer.Mana += ManaGainAmount;
        if(consumer.Mana > consumer.MaxMana) {
            consumer.Mana = consumer.MaxMana;
        }
        return true;
    }
}