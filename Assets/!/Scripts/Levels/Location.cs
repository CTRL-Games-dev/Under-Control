using System.Collections.Generic;
using UnityEngine;
public abstract class Location
{
    public int Width, Height;
    public int X, Y;
    public abstract void FindLocation(List<Location> generatedLocations);
    private bool CheckLocation(List<Location> generatedLocations)
    {
        foreach(var l in generatedLocations)
        {
            if(X+Width > l.X && X < l.X) return false;
            if(Y+Height > l.Y && Y < l.Y) return false;
        }
        return true;
    }
    public abstract void GenerateLocation();
}

// === Forest ===

public class ForestPortal : Location
{
    private GameObject _portalPrefab;
    public override void FindLocation(List<Location> generatedLocations)
    {
        string portalPath = "Prefabs/Forest/Portal";

        _portalPrefab = Resources.Load<GameObject>(portalPath);
        
        // TODO find a way to properly set up width and height
        Width = 1;
        Height = 1;

        X = Y = 0;
    }

    public override void GenerateLocation()
    {
        var gm = GameObject.Instantiate(_portalPrefab, new(X, 0, Y), Quaternion.identity);
    }
}

public class DummyLocation : Location
{
    public DummyLocation(Vector2 dimensions, Vector2 position)
    {
        this.Width = (int)dimensions.x;
        this.Height = (int)dimensions.y;

        this.X = (int)position.x;
        this.Y = (int)position.y;
    }
    public override void FindLocation(List<Location> generatedLocations)
    {   
        
    }


    public override void GenerateLocation()
    {
        Debug.Log("Generated dummy location");
    }
}