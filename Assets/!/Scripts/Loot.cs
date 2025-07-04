using System;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour {
    [Serializable]
    public struct LootData {
        public ItemData Item;
        public int MinAmount;
        public int MaxAmount;

        public float MinPowerScale;
        public float MaxPowerScale;

        [Range(0, 1)]
        public float Chance;
    }

    public bool DropOnDeath = true;
    public List<LootData> PossibleLoot = new List<LootData>();

    void Awake() {
        if(DropOnDeath) {
            if(TryGetComponent(out LivingEntity livingEntity)) {
                livingEntity.OnDeath.AddListener(Drop);
            }
        }
    }

    public void Drop() {
        foreach(LootData lootData in PossibleLoot) {
            if(UnityEngine.Random.Range(0f, 1f) > lootData.Chance) {
                continue;
            }

            if(lootData.MaxAmount < lootData.MinAmount) {
                Debug.LogError("LootData.MaxAmount < LootData.MinAmount");
                continue;
            }

            int amount = UnityEngine.Random.Range(lootData.MinAmount, lootData.MaxAmount);
            if(amount == 0) {
                continue;
            }

            float powerScale = UnityEngine.Random.Range(lootData.MinPowerScale, lootData.MaxPowerScale);

            ItemEntity.Spawn(lootData.Item, amount, transform.position, powerScale);
        }
    }
}