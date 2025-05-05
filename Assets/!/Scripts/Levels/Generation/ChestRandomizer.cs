using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Chest))]
public class ChestRandomizer : MonoBehaviour
{
    [Range(0,1)]
    public float ChanceToSpawn = 1;
    public List<ChestItemData> possibleItems;
    public EnemySpawner Spawner;
    void Awake()
    {
        gameObject.SetActive(false);
        if(UnityEngine.Random.Range(0f, 1f) <= ChanceToSpawn)
        {
            Spawner.DefeatedEnemies.AddListener(spawnChest);
            setLoot();
        } else {
            Destroy(gameObject);
        }
    }

    private void spawnChest()
    {
        gameObject.SetActive(true);
    }
    private void setLoot()
    {
        float influence = GameManager.Instance.TotalInfluence;
        float influenceDelta = GameManager.Instance.InfluenceDelta;

        Chest chest = GetComponent<Chest>();

        foreach(var i in possibleItems)
        {
            if(influence < i.MinInfluence) continue;
            if(influenceDelta < i.MinInfluenceDelta) continue;

            bool ifSpawned = UnityEngine.Random.Range(0f, 1f) <= ChanceToSpawn;

            if(!ifSpawned) continue;

            int quantity = (int)(UnityEngine.Random.Range(0, i.MaxQuantity) * GameManager.Instance.GetInfluenceModifier());
            quantity = Math.Max(quantity, 1);
            quantity = (int)Math.Min(quantity, i.MaxQuantity);

            float powerScale = (UnityEngine.Random.Range(0f, 0.33f) * GameManager.Instance.GetInfluenceModifier()) + 0.75f;

            bool spotFound = chest.Inventory.AddItem(i.item, quantity, powerScale);
            if(!spotFound) Debug.Log($"Could not find empty spot for item {i.item} of quantity {quantity}");
        }
    }
}