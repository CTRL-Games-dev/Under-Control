using System;
using System.Collections.Generic;
using UnityEngine;

public struct Area
{
    public Vector2 Dimensions;
    public Vector2 TopLeftCorner;
    public Area(Vector2 dimensions)
    {
        TopLeftCorner = new(0,0);
        Dimensions = dimensions;
    }
    public Area(Vector2 dimensions, Vector2 topLeftCorner)
    {
        TopLeftCorner = topLeftCorner;
        Dimensions = dimensions;
    }
    public Area( float width, float height, float x, float y)
    {
        TopLeftCorner = new(x,y);
        Dimensions = new(width, height);
    }
    public static Area AreaFromCenter(Vector2 center, Vector2 dimensions)
    {
        Vector2 topLeftCorner = new(center.x - (dimensions.x/2), center.y - (dimensions.y/2));
        return new
        (
            topLeftCorner,
            dimensions
        );
    }

    // Useful functions
    public void SetDimensions(float width, float height)
    {
        Dimensions = new(width,height);
    }
    public void SetCenter(Vector2 center)
    {
        TopLeftCorner = center - (Dimensions/2);
    }
    public Vector2 GetCenter()
    {
        return new(TopLeftCorner.x + (Dimensions.x/2), TopLeftCorner.y + (Dimensions.y/2));
    }
    // This function only works, if areas (rectangles) have the same rotation
    public bool IsOverlapping(Area other)
    {
        Vector2 center = GetCenter();
        Vector2 centerOther = other.GetCenter();

        float diffX = Math.Abs(center.x - centerOther.x);
        float diffY = Math.Abs(center.y - centerOther.y);

        float minDiffX = (Dimensions.x/2) + (other.Dimensions.x/2);
        float minDiffY = (Dimensions.y/2) + (other.Dimensions.y/2);

        return diffX < minDiffX && diffY < minDiffY;
    }

    public bool IsOverlapping(Area[] others)
    {
        for(int i = 0; i < others.Length; i++)
        {
            Area other = others[i];

            Vector2 center = GetCenter();
            Vector2 centerOther = other.GetCenter();

            float halfWidth = Dimensions.x / 2;
            float halfHeight = Dimensions.y / 2;

            float halfWidthOther = other.Dimensions.x / 2;
            float halfHeightOther = other.Dimensions.y / 2;

            float diffX = Math.Abs(center.x - centerOther.x);
            float diffY = Math.Abs(center.y - centerOther.y);

            float minDiffX = (Dimensions.x/2) + (other.Dimensions.x/2);
            float minDiffY = (Dimensions.y/2) + (other.Dimensions.y/2);

            if(diffX < halfWidth + halfWidthOther && diffY < halfHeight + halfHeightOther) return true;
        }
        return false;
    }
    public Vector2[,] GetEdges()
    {
        Vector2[,] edges = new Vector2[4,2];

        Vector2 p1 = new Vector2(TopLeftCorner.x, TopLeftCorner.y);
        Vector2 p2 = new Vector2(TopLeftCorner.x + Dimensions.x, TopLeftCorner.y);
        Vector2 p3 = new Vector2(TopLeftCorner.x + Dimensions.x, TopLeftCorner.y + Dimensions.y);
        Vector2 p4 = new Vector2(TopLeftCorner.x, TopLeftCorner.y + Dimensions.y);

        edges[0,0] = p1;
        edges[0,1] = p2;

        edges[1,0] = p2;
        edges[1,1] = p3;

        edges[2,0] = p3;
        edges[2,1] = p4;

        edges[3,0] = p4;
        edges[3,1] = p1;


        return edges;
    }
}
public abstract class Location
{
    public string Name;
    public GameObject SpawnedInstance = null;
    public Area LocationRectangle;
    public abstract void GenerateLocation(GameObject parent, WorldData worldData);

    // These are used to avoid one liners like "LocationRectangle.Dimensions.x"
    public Vector2 GetTileGridCenter(Vector2 offset)
    {
        return LocationRectangle.GetCenter() - offset;
    }
    public Vector2 GetTileGridCorner(Vector2 offset)
    {
        return LocationRectangle.TopLeftCorner - offset;
    }
    public Vector2 GetWorldCenter(Vector2 offset, float scale)
    {
        return GetTileGridCenter(offset) * scale;
    }
    public Vector2 GetWorldCorner(Vector2 offset, float scale)
    {
        return GetTileGridCorner(offset) * scale;
    }
    public Vector2 GetAbsoluteCenter()
    {
        return LocationRectangle.GetCenter();
    }
    public Vector2 GetAbsoluteCorner()
    {
        return LocationRectangle.TopLeftCorner;
    }
    public int GetWidth()
    {
        return (int)LocationRectangle.Dimensions.x;
    }
    public int GetHeight()
    {
        return (int)LocationRectangle.Dimensions.y;
    }
}

// === Forest ===

public class ForestPortal : Location
{
    private GameObject _portalPrefab;
    private bool _open;

    public ForestPortal(bool open)
    {
        Name = "Forest Portal";
        string portalPath = "Prefabs/Forest/ForestPortal";

        _portalPrefab = Resources.Load<GameObject>(portalPath);
        _open = open;
        
        LocationRectangle = new(new(3,3));
    }

    public override void GenerateLocation(GameObject parent, WorldData worldData)
    {
        Vector2 center = GetTileGridCenter(worldData.Offset);

        Vector3 pos = (new Vector3(center.x, 0, center.y) * worldData.Scale) + new Vector3(0.5f, 2.04f, 0.5f);
         
        SpawnedInstance = GameObject.Instantiate(_portalPrefab, pos, Quaternion.identity, parent.transform);
        SpawnedInstance.GetComponentInChildren<Portal>().EnablePortal(_open);
    }
}

public class DummyLocation : Location
{
    public DummyLocation(Vector2 dimensions, Vector2 position)
    {
        Name = "Dummy Location";
        LocationRectangle = new(dimensions, position);
    }

    public override void GenerateLocation(GameObject parent, WorldData worldData)
    {
        
    }
}

public class Medow : Location
{
    public Medow()
    {
        Name = "Medow";

        int width = UnityEngine.Random.Range(4, 5);
        int height = UnityEngine.Random.Range(4, 5);

        LocationRectangle = new(new(width, height));
    }
    public override void GenerateLocation(GameObject parent, WorldData worldData)
    {
        Vector2 corner = GetTileGridCorner(worldData.Offset);

        List<Vector2> occupiedSpace = new();

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

public class ForestBossArena : Location
{
    public ForestBossArena()
    {
        Name = "Forest Boss Arena";
        
        LocationRectangle = new(new(11, 11));
    }

    public override void GenerateLocation(GameObject parent, WorldData worldData)
    {
        GameObject boar = Resources.Load<GameObject>("Prefabs/Forest/Enemies/Boss");
        Vector2 center = GetTileGridCenter(worldData.Offset);

        GameObject boarInstance = GameObject.Instantiate(boar, new Vector3(center.x, 0.2f, center.y) * worldData.Scale, Quaternion.identity);
   
        LivingEntity boarLivingEntity = boarInstance.GetComponent<LivingEntity>();
        boarLivingEntity.OnDeath.AddListener(() => UICanvas.Instance.OpenVideoPlayer());
    }
}