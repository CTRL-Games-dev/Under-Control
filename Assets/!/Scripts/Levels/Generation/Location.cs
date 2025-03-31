using UnityEngine;

public abstract class Location : MonoBehaviour 
{
    [HideInInspector] public Vector2 LocationCenterInWorld = Vector2.zero;
    public float Width, Height;
}