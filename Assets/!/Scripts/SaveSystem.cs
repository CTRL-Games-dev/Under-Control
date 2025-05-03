using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();
    [Serializable]
    public struct SaveData
    {
        public HumanoidInventory.HumanoidInventorySaveData InventorySaveData;//idk czemu musze robic humanoidinventory. ; wczesniej tak nie musialem :/
        public Player.PlayerSaveData PlayerSaveData;
    }
    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/savedata" + ".save";
        return saveFile;
    }
    public static bool CheckIfSaveFileExists()
    {
        return File.Exists(SaveFileName());
    }
    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    private static void HandleSaveData()
    {
        Player.Instance.GetComponent<Player>().Save(ref _saveData.PlayerSaveData);
        Player.Instance.GetComponent<HumanoidInventory>().Save(ref _saveData.InventorySaveData);
    }
    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        LoadInventory();
        Player.Instance.Load(_saveData.PlayerSaveData);
        GameManager.Instance.WeaponTile.UpdateInvTile();
    }
    private static void LoadInventory(){
        HumanoidInventory inventory = Player.Instance.GetComponent<LivingEntity>().Inventory as HumanoidInventory;
        inventory.Load(_saveData.InventorySaveData);
        GameManager.Instance.InventoryPanel.GetComponent<InventoryPanel>().ChangeCurrentInventory(inventory.ItemContainer);

        Player.Instance.WeaponHolder.UpdateWeapon(_saveData.InventorySaveData.Weapon);
        LoadEquipmentItem(_saveData.InventorySaveData.Armor, GameManager.Instance.ArmorTile);
        LoadEquipmentItem(_saveData.InventorySaveData.Weapon, GameManager.Instance.WeaponTile);
        LoadEquipmentItem(_saveData.InventorySaveData.Amulet, GameManager.Instance.AmuletTile);
    }
    private static void LoadEquipmentItem(InventoryItem item, InvTileEquipment tile){
        tile.PickUpItem();
        if(Player.UICanvas.SelectedItemUI.InventoryItem != null) Player.UICanvas.SelectedItemUI.InventoryItem = null;
        tile.UpdateInvTile();
        tile.PlaceItem(item);
        tile.UpdateInvTile();
    }
}
