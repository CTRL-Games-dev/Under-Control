using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainRandomizer : MonoBehaviour
{
    
    [Header("Type of randomization")]
    public bool RandomizeTrees = true;
    //public bool RandomizeRocks = true;
    [Header("Custom holders")]
    public Transform TreeHolder;
    public Transform RockHolder;
    void Start()
    {
        if(RandomizeTrees) randomizeTrees();
        //if(RandomizeRocks) randomizeRocks();
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
            Vector3 worldPos = child.position;
            Vector3 worldScale = child.lossyScale;
            Quaternion worldRot = child.rotation;


            Destroy(child.gameObject);

            GameObject newTree = Instantiate(treeTypes[Random.Range(0, treeTypes.Length)], child);
            newTree.transform.SetParent(TreeHolder);
            newTree.transform.position = worldPos;
            newTree.transform.localScale = Vector3.one;

            // I hate scaling and those, who created these models
            Vector3 parentScale = TreeHolder.lossyScale;
            newTree.transform.localScale = new Vector3(
                worldScale.x / parentScale.x,
                worldScale.y / parentScale.y,
                worldScale.z / parentScale.z
            );

            newTree.transform.eulerAngles = new(worldRot.x, Random.Range(0f, 359f), worldRot.z);
        }
    }

    // Does not work
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
            Vector3 worldPos = child.position;
            Vector3 worldScale = child.lossyScale;
            Quaternion worldRot = child.rotation;

            Destroy(child.gameObject);

            GameObject newRock = Instantiate(rockTypes[Random.Range(0, rockTypes.Length)], child);
            newRock.transform.SetParent(RockHolder);
            newRock.transform.position = worldPos;

            newRock.transform.localScale = Vector3.one;

            // I hate scaling and those, who created these models
            Vector3 parentScale = TreeHolder.lossyScale;
            newRock.transform.localScale = new Vector3(
                worldScale.x / parentScale.x,
                worldScale.y / parentScale.y,
                worldScale.z / parentScale.z
            );

            newRock.transform.eulerAngles = new(worldRot.x, Random.Range(0f, 359f), worldRot.z);
        }
    }

}
