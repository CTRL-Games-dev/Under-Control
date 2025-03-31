using System.Collections.Generic;
using UnityEngine;

public class ForestPortalLocation : Location
{
    private GameObject _portalPrefab;
    private bool _open;

    private void Avake(bool open)
    {
        Name = "Forest Portal";
        string portalPath = "Prefabs/Forest/ForestPortal";

        _portalPrefab = Resources.Load<GameObject>(portalPath);
        _open = open;
        
        LocationRectangle = new(new(3,3));
    }

    public override void InitLocation(GameObject parent, WorldData worldData)
    {
        
    }
}