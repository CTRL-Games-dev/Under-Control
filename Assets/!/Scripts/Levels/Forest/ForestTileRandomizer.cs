using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ForestTileRandomizer : MonoBehaviour, TileRandomizer
{
    [SerializeField] public List<GameObject> _trees;
    // This function should be called before any scaling is done
    public void RandomizeTile()
    {
        foreach(var t in _trees)
        {
            float x = (float)UnityEngine.Random.Range(-500, 500) / (1000f);
            float y = (float)UnityEngine.Random.Range(-500, 500) / (1000f);

            t.transform.localPosition += new Vector3(x, 0, y);
            t.transform.eulerAngles = new Vector3(-90, UnityEngine.Random.Range(0, 360), 0);
        }
    }
}
