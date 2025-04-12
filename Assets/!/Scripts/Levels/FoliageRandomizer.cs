using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TreeRandomizer : MonoBehaviour
{
    
    [Header("Trees")]
    public bool RandomizeTrees = true;
    public GameObject TreesParentObject;
    private GameObject[] TreeTypes;
    public float TreePercentageSizeVariance = 7f;
    [Header("Rocks")]
    public bool RandomizeRocks = true;
    public GameObject RocksParentObject;
    private GameObject[] RockTypes;
    public float RockPercentageSizeVariance = 7f;
    void Start()
    {
        if (RandomizeTrees)
        {
            TreeTypes = Resources.LoadAll<GameObject>("Prefabs/Forest/Trees");
            foreach (Transform child in TreesParentObject.transform)
            {
                foreach (Transform model in child.transform) Destroy(model.gameObject);

                GameObject newTree = Instantiate(TreeTypes[Random.Range(0, TreeTypes.Length)], child);
                newTree.transform.position = child.transform.position;
                newTree.transform.eulerAngles = new(0, UnityEngine.Random.Range(0f, 359f));
                newTree.transform.localScale *= Random.Range(0.95f, 1.05f);

            }
        }
        if (RandomizeRocks)
        {
            RockTypes = Resources.LoadAll<GameObject>("Prefabs/Forest/Rocks");
            foreach (Transform child in RocksParentObject.transform)
            {
                foreach (Transform model in child.transform) Destroy(model.gameObject);

                GameObject newTree = Instantiate(RockTypes[Random.Range(0, RockTypes.Length)], child);
                newTree.transform.position = child.transform.position;
                newTree.transform.eulerAngles = new(0, UnityEngine.Random.Range(0f, 359f));
                newTree.transform.localScale *= Random.Range(1- TreePercentageSizeVariance / 100, 1+ TreePercentageSizeVariance / 100);

            }
        }

    }

}
