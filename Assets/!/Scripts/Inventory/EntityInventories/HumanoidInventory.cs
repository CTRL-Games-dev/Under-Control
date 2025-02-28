using System;
using UnityEngine;

[Serializable]
public class HumanoidInventory : SimpleInventory
{
    public HelmetItemData Helmet;
    public ChestplateItemData Chestplate;
    public LeggingsItemData Leggings;
    public BootsItemData Boots;
    public AmuletItemData Amulet;
    public RingItemData Ring;

    [SerializeField]
    private WeaponItemData leftHand;
    public WeaponItemData LeftHand { get => leftHand; }

    [SerializeField]
    private WeaponItemData rightHand;
    public WeaponItemData RightHand { get => rightHand; }

    #region Weapon

    public WeaponItemData UnequipLeftHand() {
        ValidateWeapon();

        if(leftHand == null) {
            throw new Exception("Left hand is not equipped");
        }

        if(leftHand.Twohanded) {
            return unequipTwoHanded();
        }

        var item = leftHand;

        leftHand = null;

        return item;
    }

    public WeaponItemData UnequipRightHand() {
        ValidateWeapon();

        if(rightHand == null) {
            throw new Exception("Right hand is not equipped");
        }

        if(rightHand.Twohanded) {
            return unequipTwoHanded();
        }

        var item = rightHand;

        rightHand = null;

        return item;
    }

    private WeaponItemData unequipTwoHanded() {
        if(leftHand == null || rightHand == null) {
            throw new Exception("Twohanded weapon is not equipped");
        }

        var item = leftHand;

        leftHand = null;
        rightHand = null;

        return item;
    }

    public bool CanEquipLeftHand(WeaponItemData item) {
        if(leftHand != null) {
            return false;
        }

        if(!item.Twohanded) {
            return true;
        }

        if(rightHand != null) {
            return false;
        }

        return true;
    }

    public bool EquipLeftHand(WeaponItemData item) {
        ValidateWeapon();

        if (leftHand != null) {
            return false;
        }

        if(item.Twohanded) {
            return equipTwoHanded(item);
        }
       
        leftHand = item;

        return true;
    }

    public bool CanEquipRightHand(WeaponItemData item) {
        if(rightHand != null) {
            return false;
        }

        if(!item.Twohanded) {
            return true;
        }

        if(leftHand != null) {
            return false;
        }

        return true;
    }
    
    public bool EquipRightHand(WeaponItemData item) {
        ValidateWeapon();

        if (rightHand != null) {
            return false;
        }

        if(item.Twohanded) {
            return equipTwoHanded(item);
        }
     
        rightHand = item;

        return true;
    }

    public bool HasTwohandedWeapon() {
        if(leftHand == null || rightHand == null) {
            return false;
        }

        return leftHand.Twohanded && rightHand.Twohanded;
    }

    public bool CanEquipTwoHanded() {
        return leftHand == null && rightHand == null;
    }

    public void ValidateWeapon() {
        // Check for twohanded weapon
        var hasLeftTwohanded = leftHand != null && leftHand.Twohanded;
        var hasRightTwohanded = rightHand != null && rightHand.Twohanded;
        if(hasLeftTwohanded && hasRightTwohanded) {
            if(leftHand != rightHand) {
                throw new Exception("Left and right hand weapons are not the same though they are twohanded");
            }

            return;
        }

        if(hasLeftTwohanded && !hasRightTwohanded) {
            throw new Exception("Left hand weapon is twohanded but right hand is not");
        }

        if(!hasLeftTwohanded && hasRightTwohanded) {
            throw new Exception("Right hand weapon is twohanded but left hand is not");
        }
    }

    public bool equipTwoHanded(WeaponItemData item) {
        if(leftHand != null || rightHand != null) {
            return false;
        }

        leftHand = item;
        rightHand = item;

        return true;
    }

    #endregion
}