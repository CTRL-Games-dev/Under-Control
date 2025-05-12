using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Chest))]
public class ChestRandomizer : MonoBehaviour
{
    [Range(0,1)]
    public float ChanceToSpawn = 1;
    public SpawnItemData[] possibleItems;
    public EnemySpawner Spawner;
    public VisualEffect MorphVFX;
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
        MorphVFX.Play();

        StartCoroutine(stopEfect());
    }
    private IEnumerator stopEfect() {
        yield return new WaitForSeconds(1.5f);
        MorphVFX.Stop();
        MorphVFX.SendEvent("StopOrbs");
    }
    private void setLoot()
    {
        SimpleInventory inventory = GetComponent<SimpleInventory>();
        ItemRandomizer.SetRandomItems(possibleItems, inventory, MinItems, MaxItems);
    }
}