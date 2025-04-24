using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PortalInForest : Portal
{
    public Material PortalInsideLow;
    public Material PortalInsideMedium;
    public Material PortalInsideHigh;
    public Material PortalVekthar;
    public List<MeshRenderer> Renderers;
    protected override void setInfluence()
    {
        float maxDelta = GameManager.MaxInfluenceDelta;
        float minDelta = GameManager.MinInfluenceDelta;
        float influenceDelta = UnityEngine.Random.Range(minDelta, maxDelta);
        Influence = influenceDelta + GameManager.Instance.TotalInfluence;

        Material currentMaterial;
        float step = (maxDelta-minDelta) / 3;

        if(Influence > 100) {
            _dimension = Dimension.FOREST_VECTOR;
            currentMaterial = PortalVekthar;
        }
        else if(influenceDelta < minDelta + step) {
            currentMaterial = PortalInsideLow;
        }
        else if(influenceDelta >= minDelta + step && influenceDelta < minDelta + (step * 2)) {
            currentMaterial = PortalInsideMedium;
        }
        else {
            currentMaterial = PortalInsideHigh;
        }

        foreach(var r in Renderers) {
            r.material = currentMaterial;
        }
    }
}
