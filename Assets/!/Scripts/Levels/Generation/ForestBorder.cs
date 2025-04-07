using System.Collections.Generic;
using UnityEngine;

public class ForestBorder : Location 
{
    void Awake()
    {
        transform.eulerAngles = new(0,UnityEngine.Random.Range(0f, 359f));
        transform.position += new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), 0, UnityEngine.Random.Range(-0.1f, 0.1f));
    }
}