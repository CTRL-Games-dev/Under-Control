using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainRandomizer : MonoBehaviour
{
    
    [Header("Type of randomization")]
    public bool RandomizeTrees = true;
    public bool RandomizeRocks = true;
    [Header("Custom holders")]
    public Transform TreeHolder;
    public Transform RockHolder;
    void Start()
    {
        if(RandomizeTrees) randomizeTrees();
        if(RandomizeRocks) randomizeRocks();
    }

    private void randomizeTrees()
    {
        GameObject[] treeTypes = Resources.LoadAll<GameObject>("Prefabs/Forest/Trees");

        if(TreeHolder == null) 
        {
            TreeHolder = transform.Find("Trees");
            if(TreeHolder == null)
            {
                Debug.LogError("Randomization of trees was set to \"true\", however cannot find trees parent object.");
                return;
            }
        }

        List<Transform> children = new();

        foreach (Transform child in TreeHolder)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            Transform transformCopy = child.transform;
            Destroy(child.gameObject);

            GameObject newTree = Instantiate(treeTypes[Random.Range(0, treeTypes.Length)], child);
            newTree.transform.SetParent(TreeHolder);
            newTree.transform.position = transformCopy.position;
            newTree.transform.localScale = transformCopy.localScale;

            newTree.transform.eulerAngles = new(0, UnityEngine.Random.Range(0f, 359f));
        }
    }

    private void randomizeRocks()
    {
        GameObject[] rockTypes = Resources.LoadAll<GameObject>("Prefabs/Forest/Rocks");

        if(RockHolder == null) 
        {
            RockHolder = transform.Find("Rocks");
            if(RockHolder == null)
            {
                Debug.LogError("Randomization of rocks was set to \"true\", however cannot find rocks parent object.");
                return;
            }
        }

        List<Transform> children = new();

        foreach (Transform child in RockHolder)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            Transform transformCopy = child.transform;
            Destroy(child.gameObject);

            GameObject newRock = Instantiate(rockTypes[Random.Range(0, rockTypes.Length)], child);
            newRock.transform.SetParent(RockHolder);
            newRock.transform.position = transformCopy.position;
            newRock.transform.localScale = transformCopy.localScale;

            newRock.transform.eulerAngles = new(0, UnityEngine.Random.Range(0f, 359f));
        }
    }

}
