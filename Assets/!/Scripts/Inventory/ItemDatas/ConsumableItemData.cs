using System.Collections.Generic;

public abstract class ConsumableItemData : ItemData {
    public List<ItemData> Ingredients = new List<ItemData>();
    public abstract bool Consume(LivingEntity consumer); //returns false if cant apply consumable effect
}