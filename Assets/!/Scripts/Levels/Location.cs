using System.Collections.Generic;
using UnityEngine;
public abstract class Location
{
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
    public void SetTileCenter(Vector2 center)
    {
        X = (int)(center.x) - (TileWidth/2) - (int)wd.Offset.x;
        Y = (int)(center.y) - (TileWidth/2) - (int)wd.Offset.y;
    }

    public Vector2 GetAbsoluteCenter()
    {
        return (new Vector2(X + (TileWidth/2), Y + (TileHeight/2)) - wd.Offset) * wd.Scale;
    }
    // public void SetAbsoluteCenter(Vector2 center)
    // {
    //     X = (int)(center.x) - (TileWidth/2) - (int)wd.Offset.x;
    //     Y = (int)(center.y) - (TileWidth/2) - (int)wd.Offset.y;
    // }
}

// === Forest ===

public class ForestPortal : Location
{
    private GameObject _portalPrefab;
    public ForestPortal()
    {
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
         
        var gm = GameObject.Instantiate(_portalPrefab, pos, Quaternion.identity, parent.transform);
    }
}

public class DummyLocation : Location
{
    public DummyLocation(Vector2 dimensions, Vector2 position)
    {
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
        TileWidth = Random.Range(20, 22);
        TileHeight = Random.Range(20, 22);
    }

    public override void GenerateLocation(GameObject parent, WorldData worldData)
    {
        // throw new System.NotImplementedException();
    }
}