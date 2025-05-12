using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class ItemRandomizer {
    public static void SetRandomItems(SpawnItemData[] items, SimpleInventory inventory, int minAmount, int maxAmount) {
        float influence = GameManager.Instance.TotalInfluence;
        float influenceDelta = GameManager.Instance.InfluenceDelta;

        var shuffledItems = FluffyUtils.ShuffleList(items);
        shuffledItems = shuffledItems
            .Where(x => influence >= x.MinInfluence)
            .Where(x => influenceDelta >= x.MinInfluenceDelta)
            .ToList();
        int numberOfItems = UnityEngine.Random.Range(minAmount, maxAmount);

        for(int i = 0; i < numberOfItems; i++) {
            SpawnItemData item = shuffledItems[i];
            int quantity = (int)(UnityEngine.Random.Range(0, item.MaxQuantity) * GameManager.Instance.GetInfluenceModifier());
            quantity = Math.Max(quantity, 1);
            quantity = (int)Math.Min(quantity, item.MaxQuantity);

            float powerScale = GetPowerScale();

            bool spotFound = inventory.AddItem(item.item, quantity, powerScale);
            if(!spotFound) {
                // List<InventoryItem> itemss = inventory.GetItems();
                // foreach(InventoryItem item in itemss) {
                //     Debug.Log($"{item.Amount} {item.Rotated} {item.ItemData.DisplayName} {item.Position} {item.PowerScale} {item.ItemData.Size}");
                // }
                Debug.Log($"Could not find empty spot for item {item.item} of quantity {quantity}");
            }
        }
    }

    public static float GetPowerScale() {
        return (UnityEngine.Random.Range(0f, 0.25f) * GameManager.Instance.GetInfluenceModifier()) + 0.75f;
    }
    public static float GetStartPowerScale() {
        return (UnityEngine.Random.Range(0f, 0.25f) * GameManager.Instance.GetInfluenceModifier()) + 0.5f;
    }
}