using System;

[Serializable]
public class EntityInventory : ItemContainer
{
    // TODO: These WONT be InventoryItems soon, that's just a placeholder
    // Don't rely on these being InventoryItems
    public InventoryItem Helmet;
    public InventoryItem Chestplate;
    public InventoryItem Boots;
    public InventoryItem Amulet;
    public InventoryItem Ring;
    public InventoryItem LeftHand;
    public InventoryItem RightHand;

    public EntityInventory() : base(4, 4) {}

    public EntityInventory(int width, int height) : base(width, height) {}
}