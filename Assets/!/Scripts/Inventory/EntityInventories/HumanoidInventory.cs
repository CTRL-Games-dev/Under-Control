using System;

[Serializable]
public class HumanoidInventory : SimpleInventory
{
    private AmuletItemData _amulet;
    public AmuletItemData Amulet
    {
        get => _amulet;
        set
        {
            _amulet = value;
            OnInventoryChanged.Invoke();
        }
    }

    private ArmorItemData _armor;
    public ArmorItemData Armor
    {
        get => _armor;
        set
        {
            _armor = value;
            OnInventoryChanged.Invoke();
        }
    }

    private WeaponItemData _weapon;
    public WeaponItemData Weapon
    {
        get => _weapon;
        set
        {
            _weapon = value;
            OnInventoryChanged.Invoke();
        }
    }
}