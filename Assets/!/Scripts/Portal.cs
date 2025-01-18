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
        if(isOpen)
        {
            Debug.Log("Player entered portal to: " + dimension);
            playerEnteredPortal.Invoke(dimension);
        }
    }
}
