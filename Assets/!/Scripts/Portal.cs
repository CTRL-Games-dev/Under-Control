using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    public bool isOpen = true;
    public Dimension dimension = Dimension.HUB;
    public UnityEvent<Dimension> playerEnteredPortal;

    void OnTriggerEnter(Collider other)
    {
        playerEnteredPortal.Invoke(dimension);

        if(isOpen) 
        {
            playerEnteredPortal.Invoke(dimension);
        }
    }
}
