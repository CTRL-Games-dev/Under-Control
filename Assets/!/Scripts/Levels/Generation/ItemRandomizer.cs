using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class ItemRandomizer {
    public static void SetRandomItems(SpawnItemData[] items, SimpleInventory inventory) {
        float influence = GameManager.Instance.TotalInfluence;
        float influenceDelta = GameManager.Instance.InfluenceDelta;

        foreach(var i in items) {
            if(influence < i.MinInfluence) continue;
            if(influenceDelta < i.MinInfluenceDelta) continue;
            if(UnityEngine.Random.Range(0f, 1f) > i.ChanceToSpawn) continue;

            int quantity = (int)(UnityEngine.Random.Range(0, i.MaxQuantity) * GameManager.Instance.GetInfluenceModifier());
            quantity = Math.Max(quantity, 1);
            quantity = (int)Math.Min(quantity, i.MaxQuantity);

            float powerScale = (UnityEngine.Random.Range(0f, 0.33f) * GameManager.Instance.GetInfluenceModifier()) + 0.75f;

            bool spotFound = inventory.AddItem(i.item, quantity, powerScale);
            if(!spotFound) {
                List<InventoryItem> itemss = inventory.GetItems();
                foreach(InventoryItem item in itemss) {
                    Debug.Log($"{item.Amount} {item.Rotated} {item.ItemData.DisplayName} {item.Position} {item.PowerScale} {item.ItemData.Size}");
                }
                Debug.Log($"Could not find empty spot for item {i.item} of quantity {quantity}");
            }
        }
    }

    public static float GetPowerScale() {
        return (UnityEngine.Random.Range(0f, 0.33f) * GameManager.Instance.GetInfluenceModifier()) + 0.75f;
    }
}