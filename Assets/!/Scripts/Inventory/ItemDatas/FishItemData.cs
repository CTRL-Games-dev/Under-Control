using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Itm_EffectConsumableItem", menuName = "Items/EffectConsumableItemData")]
public class EffectConsumableItemData : ConsumableItemData {
    public Effect EffectToApply;
    public override bool Consume(LivingEntity consumer) {
        if(consumer.ActiveEffects.Select(x => x.Effect).Contains(EffectToApply)) return false;
        consumer.ApplyEffect(EffectToApply);
        return true;
    }
}