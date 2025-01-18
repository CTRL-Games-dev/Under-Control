using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    public bool IsOpen = true;
    public Dimension Dimension = Dimension.HUB;
    public UnityEvent<Dimension> PlayerEnteredPortal;

    void OnTriggerEnter(Collider other)
    {
        if(IsOpen) 
        {
            Debug.Log("Player entered portal to: " + Dimension);
            PlayerEnteredPortal.Invoke(Dimension);
        }
    }
}
