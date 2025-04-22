using System;

[Serializable]
public class HumanoidInventory : SimpleInventory
{
    private InventoryItem<AmuletItemData> _amulet;
    public InventoryItem<AmuletItemData> Amulet
    {
        get => _amulet;
        set
        {
            _amulet = value;
            OnInventoryChanged.Invoke();
        }
    }

    private InventoryItem<ArmorItemData> _armor;
    public InventoryItem<ArmorItemData> Armor
    {
        get => _armor;
        set
        {
            _armor = value;
            OnInventoryChanged.Invoke();
        }
    }

    private InventoryItem<WeaponItemData> _weapon;
    public InventoryItem<WeaponItemData> Weapon
    {
        get => _weapon;
        set
        {
            _weapon = value;
            OnInventoryChanged.Invoke();
        }
    }
}