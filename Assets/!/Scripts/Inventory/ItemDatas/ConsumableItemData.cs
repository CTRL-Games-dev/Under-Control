public abstract class ConsumableItemData : ItemData {
    public float CooldownTime;
    
    public abstract void Consume(LivingEntity consumer);
}