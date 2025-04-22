using System;
using UnityEngine;

[Serializable]
public class HumanoidInventory : SimpleInventory
{
    [SerializeField]
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

    [SerializeField]
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

    [SerializeField]
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