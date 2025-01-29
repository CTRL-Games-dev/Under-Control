using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class ForestTileRandomizer : MonoBehaviour, TileRandomizer
{
    [SerializeField] private List<GameObject> _tree_positions; // List of tree position
    [SerializeField] private List<GameObject> _tree_variants; // List of tree variants;

    // This function should be called before any scaling is done
    // and after prefab of the tile is spawned!
    public void RandomizeTile()
    {
        float influence = GameManager.Gm.GetInfluence();
        // Material trunk = Resources.Load<Material>("Materials/Forest/TreeTrunk");
        // Material leaves = Resources.Load<Material>("Materials/Forest/TreeLeaves");

        // var defaultColor = leaves.color;
        foreach(var t in _tree_positions)
        {
            int iRand = Random.Range(0, _tree_variants.Count);
            GameObject variant = _tree_variants[iRand];
            Transform tr = t.transform;

            GameObject treesHolder = transform.Find("Trees").gameObject;
            GameObject newTree = Instantiate(variant, tr.position, tr.rotation, treesHolder.transform);
            newTree.transform.localScale = tr.localScale;

            if(influence > Random.Range(0.001f, 1f)) newTree.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(76f / 255f, 36f / 255f, 84f / 255f, 1f));
            
            float x = (float)UnityEngine.Random.Range(-500, 500) / (1000f);
            float y = (float)UnityEngine.Random.Range(-500, 500) / (1000f);

            t.transform.localPosition += new Vector3(x, 0, y);
            t.transform.eulerAngles = new Vector3(-90, UnityEngine.Random.Range(0, 360), 0);

            // Dispose of the dummy tree
            Destroy(t.gameObject);
        }
    }
}
