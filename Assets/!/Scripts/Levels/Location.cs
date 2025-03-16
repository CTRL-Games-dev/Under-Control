using System.Collections.Generic;
using UnityEngine;
public abstract class Location
{
    public string Name;
    public GameObject SpawnedInstance = null;
    public int TileWidth, TileHeight;
    public int TilePosX, TilePosY;
    public abstract void GenerateLocation(GameObject parent, WorldData worldData);
    public bool CheckLocation(List<Location> generatedLocations)
    {
        foreach(var l in generatedLocations)
        {
            if(TilePosX+TileWidth > l.TilePosX && TilePosX < l.TilePosX) return false;
            if(TilePosY+TileHeight > l.TilePosY && TilePosY < l.TilePosY) return false;
        }
        return true;
    }

    public Vector2 GetTileGridCenter(Vector2 offset)
    {
        return new Vector2(TilePosX + (TileWidth/2), TilePosY + (TileHeight/2)) - offset;
    }
    public Vector2 GetTileGridCorner(Vector2 offset)
    {
        return new Vector2(TilePosX, TilePosY) - offset;
    }
    public Vector2 GetTileCenterWithoutOffset()
    {
        return new Vector2(TilePosX + (TileWidth/2), TilePosY + (TileHeight/2));
    }
    public void SetTileCenter(Vector2 center)
    {
        TilePosX = (int)(center.x) - (TileWidth/2);
        TilePosY = (int)(center.y) - (TileWidth/2);
    }

    public Vector2 GetAbsoluteCorner(Vector2 offset, float scale)
    {
        return (new Vector2(TilePosX, TilePosY) - offset) * scale;
    }
    public Vector2 GetAbsoluteCenter(Vector2 offset, float scale)
    {
        return (new Vector2(TilePosX + (TileWidth/2), TilePosY + (TileHeight/2)) - offset) * scale;
    }
    
    public Vector2[,] GetEdges()
    {
        Vector2[,] edges = new Vector2[4,2];

        Vector2 p1 = new Vector2(TilePosX,TilePosY);
        Vector2 p2 = new Vector2(TilePosX+TileWidth,TilePosY);
        Vector2 p3 = new Vector2(TilePosX+TileWidth,TilePosY+TileHeight);
        Vector2 p4 = new Vector2(TilePosX,TilePosY+TileHeight);

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

// === Forest ===

public class ForestPortal : Location
{
    private GameObject _portalPrefab;
    public ForestPortal(bool open)
    {
        Name = "Forest Portal";
        string portalPath = "Prefabs/Forest/ForestPortal";

        _portalPrefab = Resources.Load<GameObject>(portalPath);
        _portalPrefab.GetComponentInChildren<Portal>().EnablePortal(open);
        
        TileWidth = 3;
        TileHeight = 3;
    }

    public override void GenerateLocation(GameObject parent, WorldData worldData)
    {
        Vector2 center = GetTileGridCenter(worldData.Offset);

        Vector3 pos = (new Vector3(center.x, 0, center.y) * worldData.Scale) + new Vector3(0.5f, 3.0f, 0.5f);
         
        SpawnedInstance = GameObject.Instantiate(_portalPrefab, pos, Quaternion.identity, parent.transform);
    }
}

public class DummyLocation : Location
{
    public DummyLocation(Vector2 dimensions, Vector2 position)
    {
        Name = "Dummy Location";
        this.TileWidth = (int)dimensions.x;
        this.TileHeight = (int)dimensions.y;

        this.TilePosX = (int)position.x;
        this.TilePosY = (int)position.y;
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

        TileWidth = Random.Range(5, 8);
        TileHeight = Random.Range(5, 8);
    }
    public override void GenerateLocation(GameObject parent, WorldData worldData)
    {
        Vector2 corner = GetTileGridCorner(worldData.Offset);

        List<Vector2> occupiedSpace = new();

        // Boars
        GameObject[] trees = Resources.LoadAll<GameObject>("Prefabs/Forest/Trees");
        float numberOfTrees = Random.Range(4, 8);

        for(int i = 0; i < numberOfTrees; i++)
        {
            float x = Random.Range(corner.x, corner.x + TileWidth);
            float y = Random.Range(corner.y, corner.y + TileHeight);

            Vector3 pos = new Vector3(x, 0, y) * worldData.Scale;
            GameObject treePrefab = trees[Random.Range(0,trees.Length)];
            GameObject newTree = GameObject.Instantiate(treePrefab, pos, Quaternion.identity);

            newTree.transform.eulerAngles = new(0, Random.Range(-180,180), 0);
            occupiedSpace.Add(new(pos.x, pos.y));
        }


        // Rocks
        GameObject[] rocks = Resources.LoadAll<GameObject>("Prefabs/Forest/Rocks");
        float numberOfRocks = Random.Range(10, 20);

        for(int i = 0; i < numberOfRocks; i++)
        {
            float x = Random.Range(corner.x, corner.x + TileWidth);
            float y = Random.Range(corner.y, corner.y + TileHeight);

            Vector3 pos = new Vector3(x, -0.1f, y) * worldData.Scale;
            GameObject treePrefab = rocks[Random.Range(0,trees.Length)];
            GameObject newTree = GameObject.Instantiate(treePrefab, pos, Quaternion.identity);

            newTree.transform.eulerAngles = new(0, Random.Range(-180,180), 0);
            occupiedSpace.Add(new(pos.x, pos.y));    
        }

        // Boars
        GameObject boar = Resources.Load<GameObject>("Prefabs/Forest/Enemies/Boar");

        float numberOfBoars = Random.Range(3, 6);
        for(int i = 0; i < numberOfBoars; i++)
        {
            float x = Random.Range(corner.x + 2f, corner.x + TileWidth - 2f);
            float y = Random.Range(corner.y + 2f, corner.y + TileHeight - 2f);
            float tolerance = 3.0f;

            foreach(var l in occupiedSpace)
            {
                bool flag = false;
                for(int t = 0; t < 5; t++)
                {
                    bool con1 = l.x + tolerance < x + t;
                    bool con2 = l.x - tolerance > x + t;
                    bool con3 = l.y + tolerance < y + t;
                    bool con4 = l.y - tolerance > y + t;

                    if(!(con1 && con2 && con3 && con4)) {
                        GameObject.Instantiate(boar, new Vector3(x, 0.2f, y) * worldData.Scale, Quaternion.identity);
                        flag = true;
                        break;
                    }
                }
                if(flag) break;
            }
        }
    }
}

public class ForestBossArena : Location
{
    public ForestBossArena()
    {
        Name = "Forest Boss Arena";
        
        TileWidth = 11;
        TileHeight = 11;
    }

    public override void GenerateLocation(GameObject parent, WorldData worldData)
    {
        GameObject boar = Resources.Load<GameObject>("Prefabs/Forest/Enemies/Boss");
        Vector2 center = GetTileGridCenter(worldData.Offset);

        GameObject.Instantiate(boar, new Vector3(center.x, 0.2f, center.y) * worldData.Scale, Quaternion.identity);
    }
}