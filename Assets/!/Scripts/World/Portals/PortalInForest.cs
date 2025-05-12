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
    public float influenceDelta;
    protected override void setInfluence()
    {
        float maxDelta = GameManager.MaxInfluenceDelta;
        float minDelta = GameManager.MinInfluenceDelta;
        influenceDelta = UnityEngine.Random.Range(minDelta, maxDelta);
        Influence = influenceDelta + GameManager.Instance.TotalInfluence;
    }

    public override void SetDimension(Dimension dimension) {
        _dimension = dimension;

        float maxDelta = GameManager.MaxInfluenceDelta;
        float minDelta = GameManager.MinInfluenceDelta;

        Material currentMaterial;
        float step = (maxDelta-minDelta) / 3;

        if(dimension != Dimension.FOREST) {
            if(influenceDelta < minDelta + step) {
                currentMaterial = PortalInsideLow;
            }
            else if(influenceDelta >= minDelta + step && influenceDelta < minDelta + (step * 2)) {
                currentMaterial = PortalInsideMedium;
            }
            else {
                currentMaterial = PortalInsideHigh;
            }
        } else {
            currentMaterial = PortalVekthar;
        }

        foreach(var r in Renderers) {
            r.material = currentMaterial;
        }
    }
}
