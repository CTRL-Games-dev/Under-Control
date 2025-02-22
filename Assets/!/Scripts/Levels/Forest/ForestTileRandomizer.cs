using System.Collections.Generic;
using UnityEngine;
public class ForestTileRandomizer : MonoBehaviour, TileRandomizer
{
    [SerializeField] private List<GameObject> _treePositions; // List of tree position
    [SerializeField] private List<GameObject> _treeVariants; // List of tree variants;
    [SerializeField] private List<GameObject> _stoneVariants;

    // This function should be called before any scaling is done
    // and after prefab of the tile is spawned!
    public void RandomizeTile()
    {
        float influence = GameManager.instance.GetInfluence();
        // Material trunk = Resources.Load<Material>("Materials/Forest/TreeTrunk");
        // Material leaves = Resources.Load<Material>("Materials/Forest/TreeLeaves");

        // var defaultColor = leaves.color;
        GameObject treesHolder = transform.Find("Trees").gameObject;
        foreach(var t in _treePositions)
        {
            int treeVariantIndex = Random.Range(0, _treeVariants.Count);
            GameObject variant = _treeVariants[treeVariantIndex];
            Transform tr = t.transform;

            GameObject newTree = Instantiate(variant, tr.position, tr.rotation, treesHolder.transform);
            newTree.transform.localScale = tr.localScale;

            if(influence > Random.Range(0.001f, 1f)) newTree.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(76f / 255f, 36f / 255f, 84f / 255f, 1f));
            
            float x = Random.Range(-0.5f, 0.5f);
            float y = Random.Range(-0.5f, 0.5f);

            t.transform.localPosition += new Vector3(x, 0, y);
            t.transform.eulerAngles = new Vector3(-90, UnityEngine.Random.Range(0, 360), 0);

            // Dispose of the dummy tree
            Destroy(t.gameObject);
        }

        int numberOfRocks = Random.Range(0, 3);
        GameObject rockHolder = transform.Find("RockHolder").gameObject;
        for(int i = 0; i < numberOfRocks; i++)
        {
            int stoneVariantIndex = Random.Range(0, _stoneVariants.Count);
            float stoneLocalX = Random.Range(-0.05f, 0.05f);
            float stoneLocalY = Random.Range(-0.05f, 0.05f);
            int stoneRoation = Random.Range(0, 360);

            GameObject stone = _stoneVariants[stoneVariantIndex];
            stone = Instantiate(stone, new(0,0,0), Quaternion.identity, rockHolder.transform);

            stone.transform.localPosition = new Vector3(stoneLocalX, 0, stoneLocalY);
            stone.transform.localScale = new(1,1,1);
            stone.transform.eulerAngles = new Vector3(-90, stoneRoation, 0);
        }
    }
}
