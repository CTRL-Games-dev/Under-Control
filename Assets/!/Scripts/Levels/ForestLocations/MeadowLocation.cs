using System.Collections.Generic;
using UnityEngine;

public class MeadowLocation : Location
{
    public float Width, Height;
    private void Awake()
    {
        LocationRectangle = new(new(Width, Height));
    }
    public override void InitLocation(GameObject parent, WorldData worldData)
    {
        // Vector2 corner = GetTileGridCorner(worldData.Offset);

        // List<Vector2> occupiedSpace = new();

        // GameObject[] trees = Resources.LoadAll<GameObject>("Prefabs/Forest/Trees");
        // float numberOfTrees = UnityEngine.Random.Range(4, 8);

        // for(int i = 0; i < numberOfTrees; i++)
        // {
        //     float x = UnityEngine.Random.Range(corner.x, corner.x + LocationRectangle.Dimensions.x);
        //     float y = UnityEngine.Random.Range(corner.y, corner.y + LocationRectangle.Dimensions.y);

        //     Vector3 pos = new Vector3(x, 0, y) * worldData.Scale;
        //     GameObject treePrefab = trees[UnityEngine.Random.Range(0,trees.Length)];
        //     GameObject newTree = GameObject.Instantiate(treePrefab, pos, Quaternion.identity);

        //     newTree.transform.eulerAngles = new(0, UnityEngine.Random.Range(-180,180), 0);
        //     occupiedSpace.Add(new(pos.x, pos.y));
        // }


        // // Rocks
        // GameObject[] rocks = Resources.LoadAll<GameObject>("Prefabs/Forest/Rocks");
        // float numberOfRocks = UnityEngine.Random.Range(10, 20);

        // for(int i = 0; i < numberOfRocks; i++)
        // {
        //     float x = UnityEngine.Random.Range(corner.x, corner.x + TileWidth);
        //     float y = UnityEngine.Random.Range(corner.y, corner.y + TileHeight);

        //     Vector3 pos = new Vector3(x, -0.1f, y) * worldData.Scale;
        //     GameObject treePrefab = rocks[Random.Range(0,trees.Length)];
        //     GameObject newTree = GameObject.Instantiate(treePrefab, pos, Quaternion.identity);

        //     newTree.transform.eulerAngles = new(0, Random.Range(-180,180), 0);
        //     occupiedSpace.Add(new(pos.x, pos.y));    
        // }

        // // Boars
        // GameObject boar = Resources.Load<GameObject>("Prefabs/Forest/Enemies/Boar");

        // float numberOfBoars = Random.Range(8, 20);
        // for(int i = 0; i < numberOfBoars; i++)
        // {
        //     float x = Random.Range(corner.x + 2f, corner.x + TileWidth - 2f);
        //     float y = Random.Range(corner.y + 2f, corner.y + TileHeight - 2f);
        //     float tolerance = 3.0f;

        //     foreach(var l in occupiedSpace)
        //     {
        //         bool flag = false;
        //         for(int t = 0; t < 5; t++)
        //         {
        //             bool con1 = l.x + tolerance < x + t;
        //             bool con2 = l.x - tolerance > x + t;
        //             bool con3 = l.y + tolerance < y + t;
        //             bool con4 = l.y - tolerance > y + t;

        //             if(!(con1 && con2 && con3 && con4)) {
        //                 GameObject.Instantiate(boar, new Vector3(x, 0.2f, y) * worldData.Scale, Quaternion.identity);
        //                 flag = true;
        //                 break;
        //             }
        //         }
        //         if(flag) break;
        //     }
        // }
    }
}