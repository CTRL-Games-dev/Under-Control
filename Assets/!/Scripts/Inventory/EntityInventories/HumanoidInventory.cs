using System;

[Serializable]
public class HumanoidInventory : SimpleInventory
{
    public HelmetItemData Helmet
    {
        get => helmet;
        set
        {
            helmet = value;
            OnInventoryChanged.Invoke();
        }
    }
    private HelmetItemData helmet;
    
    public ChestplateItemData Chestplate
    {
        get => chestplate;
        set
        {
            chestplate = value;
            OnInventoryChanged.Invoke();
        }
    }
    private ChestplateItemData chestplate;

    public LeggingsItemData Leggings
    {
        get => leggings;
        set
        {
            leggings = value;
            OnInventoryChanged.Invoke();
        }
    }
    private LeggingsItemData leggings;

    public BootsItemData Boots
    {
        get => boots;
        set
        {
            boots = value;
            OnInventoryChanged.Invoke();
        }
    }
    private BootsItemData boots;

    public AmuletItemData Amulet
    {
        get => amulet;
        set
        {
            amulet = value;
            OnInventoryChanged.Invoke();
        }
    }
    private AmuletItemData amulet;

    public RingItemData Ring
    {
        get => ring;
        set
        {
            ring = value;
            OnInventoryChanged.Invoke();
        }
    }
    private RingItemData ring;

    public WeaponItemData Weapon
    {
        get => weapon;
        set
        {
            weapon = value;
            OnInventoryChanged.Invoke();
        }
    }
    private WeaponItemData weapon;
}