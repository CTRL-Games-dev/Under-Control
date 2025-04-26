using System.Collections.Generic;

public abstract class ConsumableItemData : ItemData {
    public List<ItemData> Ingredients = new List<ItemData>();
    public abstract void Consume(LivingEntity consumer);
}