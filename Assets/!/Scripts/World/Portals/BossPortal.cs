using System.Collections.Generic;
using UnityEngine;

public class BossPortal : Portal
{
    protected override void setInfluence()
    {
        Influence = GameManager.Instance.TotalInfluence + 10;
    }
}