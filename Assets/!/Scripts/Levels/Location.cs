using System.Collections.Generic;
using UnityEngine;
public abstract class Location
{
    public int Width, Height;
    public int X, Y;
    public abstract void GenerateLocation(GameObject parent);
    public bool CheckLocation(List<Location> generatedLocations)
    {
        foreach(var l in generatedLocations)
        {
            if(X+Width > l.X && X < l.X) return false;
            if(Y+Height > l.Y && Y < l.Y) return false;
        }
        return true;
    }

    public Vector2 GetCenter()
    {
        return new(X + (Width/2), Y + (Height/2));
    }
    public void SetCenter(Vector2 center)
    {
        X = (int)(center.x) - (Width/2);
        Y = (int)(center.y) - (Width/2);
    }
}

// === Forest ===

public class ForestPortal : Location
{
    private GameObject _portalPrefab;
    public ForestPortal(int x, int y)
    {
        string portalPath = "Prefabs/Forest/Portal";

        _portalPrefab = Resources.Load<GameObject>(portalPath);

        this.X = x;
        this.Y = y;
        
        Width = 1;
        Height = 1;
    }

    public override void GenerateLocation(GameObject parent)
    {
        var gm = GameObject.Instantiate(_portalPrefab, new(X, 0, Y), Quaternion.identity, parent.transform);
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

    public override void GenerateLocation(GameObject parent)
    {

    }
}

public class Medow : Location
{
    public Medow()
    {
        Width = Random.Range(5, 12);
        Height = Random.Range(7, 12);
    }

    public override void GenerateLocation(GameObject parent)
    {
        // throw new System.NotImplementedException();
    }
}