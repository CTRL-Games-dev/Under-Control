using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Chest))]
public class ChestRandomizer : MonoBehaviour
{
    [Range(0,1)]
    public float ChanceToSpawn = 1;
    public SpawnItemData[] possibleItems;
    public EnemySpawner Spawner;
    public int MinItems = 1;
    public int MaxItems = 1;
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
        SimpleInventory inventory = GetComponent<SimpleInventory>();
        ItemRandomizer.SetRandomItems(possibleItems, inventory, MinItems, MaxItems);
    }
}