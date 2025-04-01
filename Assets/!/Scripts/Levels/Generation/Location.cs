using System.Collections.Generic;
using UnityEngine;

public abstract class Location : MonoBehaviour 
{
    [HideInInspector] public Vector2 LocationCenterInWorld = Vector2.zero;
    [HideInInspector] public List<Location> ConnectedLocations;
    public float Width, Height;
}