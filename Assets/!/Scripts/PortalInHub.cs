using UnityEngine;
using UnityEngine.Events;

public class PortalInHub : Portal
{
    protected override void setInfluence()
    {
        Influence = 10;
    }
}
