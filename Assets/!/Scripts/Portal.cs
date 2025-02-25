using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    public bool IsOpen = true;
    private Dimension _dimension = Dimension.HUB;
    public UnityEvent<Dimension> PlayerEnteredPortal;

    void OnTriggerEnter(Collider other)
    {
        if(IsOpen) 
        {
            Debug.Log("Player entered portal to: " + _dimension);
            PlayerEnteredPortal.Invoke(_dimension);
        }
    }
    
    public void ChangeDimension(Dimension d) {
        Debug.Log("Changed dimension to: " + d.ToString());
        _dimension = d;
    }
}
