using UnityEngine;
using UnityEngine.Events;

public class PortalInForest : Portal
{
    protected override void setInfluence()
    {
        Influence = UnityEngine.Random.Range(-2.5f, 2.5f) + 10f + GameManager.Instance.TotalInfluence;
    }
}
