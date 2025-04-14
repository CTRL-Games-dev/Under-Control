using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour 
{
    [HideInInspector] public Vector2 LocationCenterInWorld = Vector2.zero;
    [HideInInspector] public List<Location> ConnectedLocations;
    public string Name = "Default Name";
    public float Width, Height;
    public Vector2 GetTopLeftCorner()
    {
        Vector2 pos = new()
        {
            x = LocationCenterInWorld.x - (Width / 2),
            y = LocationCenterInWorld.y - (Height / 2)
        };
        return pos;
    }
    public Vector2 GetTopRightCorner()
    {
        Vector2 pos = new()
        {
            x = LocationCenterInWorld.x + (Width / 2),
            y = LocationCenterInWorld.y - (Height / 2)
        };
        return pos;
    }

    public Vector2 GetBottomLeftCorner()
    {
        Vector2 pos = new()
        {
            x = LocationCenterInWorld.x - (Width / 2),
            y = LocationCenterInWorld.y + (Height / 2)
        };
        return pos;
    }

    public Vector2 GetBottomRightCorner()
    {
        Vector2 pos = new()
        {
            x = LocationCenterInWorld.x + (Width / 2),
            y = LocationCenterInWorld.y + (Height / 2)
        };
        return pos;
    }

    public Vector3 GetTopLeftCorner3()
    {
        Vector3 pos = new()
        {
            x = LocationCenterInWorld.x - (Width / 2),
            y = 0,
            z = LocationCenterInWorld.y - (Height / 2)
        };
        return pos;
    }

    public Vector3 GetTopRightCorner3()
    {
        Vector3 pos = new()
        {
            x = LocationCenterInWorld.x + (Width / 2),
            y = 0,
            z = LocationCenterInWorld.y - (Height / 2)
        };
        return pos;
    }

    public Vector3 GetBottomLeftCorner3()
    {
        Vector3 pos = new()
        {
            x = LocationCenterInWorld.x - (Width / 2),
            y = 0,
            z = LocationCenterInWorld.y + (Height / 2)
        };
        return pos;
    }

    public Vector3 GetBottomRightCorner3()
    {
        Vector3 pos = new()
        {
            x = LocationCenterInWorld.x + (Width / 2),
            y = 0,
            z = LocationCenterInWorld.y + (Height / 2)
        };
        return pos;
    }
}