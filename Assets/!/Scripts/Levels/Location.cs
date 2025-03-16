using System.Collections.Generic;
using UnityEngine;
public abstract class Location
{
    public string Name;
    public GameObject SpawnedInstance = null;
    public int TileWidth, TileHeight;
    public int X, Y;
    public WorldData wd;
    public abstract void GenerateLocation(GameObject parent, WorldData worldData);
    public bool CheckLocation(List<Location> generatedLocations)
    {
        foreach(var l in generatedLocations)
        {
            if(X+TileWidth > l.X && X < l.X) return false;
            if(Y+TileHeight > l.Y && Y < l.Y) return false;
        }
        return true;
    }

    public Vector2 GetTileCenter()
    {
        return new Vector2(X + (TileWidth/2), Y + (TileHeight/2)) - wd.Offset;
    }
    public Vector2 GetTileCenterWithoutOffset()
    {
        return new Vector2(X + (TileWidth/2), Y + (TileHeight/2));
    }
    public void SetTileCenter(Vector2 center)
    {
        X = (int)(center.x) - (TileWidth/2) - (int)wd.Offset.x;
        Y = (int)(center.y) - (TileWidth/2) - (int)wd.Offset.y;
    }

    public Vector2 GetAbsoluteCenter()
    {
        return (new Vector2(X + (TileWidth/2), Y + (TileHeight/2)) - wd.Offset) * wd.Scale;
    }
    
    public Vector2[,] GetEdges()
    {
        Vector2[,] edges = new Vector2[4,2];

        Vector2 p1 = new Vector2(X,Y);
        Vector2 p2 = new Vector2(X+TileWidth,Y);
        Vector2 p3 = new Vector2(X+TileWidth,Y+TileHeight);
        Vector2 p4 = new Vector2(X,Y+TileHeight);

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
    public ForestPortal()
    {
        Name = "Forest Portal";
        string portalPath = "Prefabs/Forest/ForestPortal";

        _portalPrefab = Resources.Load<GameObject>(portalPath);
        
        TileWidth = 3;
        TileHeight = 3;
    }

    public override void GenerateLocation(GameObject parent, WorldData worldData)
    {
        wd = worldData;
        Vector2 center = GetTileCenter();

        Vector3 pos = (new Vector3(center.x, 0, center.y) * wd.Scale) + new Vector3(0.5f, 3.0f, 0.5f);
         
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

        this.X = (int)position.x;
        this.Y = (int)position.y;
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

        TileWidth = Random.Range(8, 20);
        TileHeight = Random.Range(8, 20);
    }

    public override void GenerateLocation(GameObject parent, WorldData worldData)
    {
        // throw new System.NotImplementedException();
    }
}