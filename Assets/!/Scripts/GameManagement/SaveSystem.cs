using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();
    private static GameManager _gm => GameManager.Instance;
    private static string _saveName = String.Empty;
    [Serializable]
    public struct SaveData
    {
        public bool IsInRun;
        public HumanoidInventory.HumanoidInventorySaveData InventorySaveData;//idk czemu musze robic humanoidinventory. ; wczesniej tak nie musialem :/
        public Player.PlayerSaveData PlayerSaveData;
        public GameManager.GlobalSaveData CardSaveData;
    }
    public static bool SetSave(string Name)
    {
        if (Name == String.Empty) return false;
        if (Name.Length > 20) return false;
        if (!Name.Contains('/') && !Name.Contains('"') && Name.Contains('.')) return false;
        _saveName = Name;
        return true;
    }
    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/" + _saveName + ".save";
        return saveFile;
    }
    public static bool CheckIfSaveFileExists()
    {
        return File.Exists(SaveFileName());
    }
    public static void SaveGame()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    private static void HandleSaveData()
    {
        Player.Instance.GetComponent<Player>().Save(ref _saveData.PlayerSaveData);
        Player.Instance.GetComponent<HumanoidInventory>().Save(ref _saveData.InventorySaveData);
        _gm.Save(ref _saveData.CardSaveData);
        _saveData.IsInRun = _gm.CurrentDimension == Dimension.CARD_CHOOSE ? true : false;
    }
    public static void LoadGame()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
        if(_saveData.IsInRun) GameManager.Instance.ChangeDimension(Dimension.CARD_CHOOSE);
    }

    private static void HandleLoadData()
    {
        LoadInventory();
        Player.Instance.Load(_saveData.PlayerSaveData);
        _gm.Load(_saveData.CardSaveData);
        _gm.WeaponTile.UpdateInvTile();
    }
    private static void LoadInventory(){
        HumanoidInventory inventory = Player.Instance.GetComponent<LivingEntity>().Inventory as HumanoidInventory;
        inventory.Load(_saveData.InventorySaveData);
        _gm.InventoryPanel.GetComponent<InventoryPanel>().ChangeCurrentInventory(inventory.ItemContainer);

        Player.Instance.WeaponHolder.UpdateWeapon(_saveData.InventorySaveData.Weapon);
        LoadEquipmentItem(_saveData.InventorySaveData.Armor, _gm.ArmorTile);
        LoadEquipmentItem(_saveData.InventorySaveData.Weapon, _gm.WeaponTile);
        LoadEquipmentItem(_saveData.InventorySaveData.Amulet, _gm.AmuletTile);
        LoadEquipmentItem(_saveData.PlayerSaveData.ConsumableItemOne, _gm.Consumable1Tile);
        LoadEquipmentItem(_saveData.PlayerSaveData.ConsumableItemTwo, _gm.Consumable2Tile);
    }
    private static void LoadEquipmentItem(InventoryItem item, InvTileEquipment tile){
        tile.PickUpItem();
        if(Player.UICanvas.SelectedItemUI.InventoryItem != null) Player.UICanvas.SelectedItemUI.InventoryItem = null;
        tile.UpdateInvTile();
        tile.PlaceItem(item);
        tile.UpdateInvTile();
    }
}
