using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    public bool IsOpen = true;
    [SerializeField] private Dimension _dimension = Dimension.HUB;
    public UnityEvent<Dimension> PlayerEnteredPortal;

    void OnTriggerEnter(Collider other)
    {
        if(IsOpen) 
        {
            Debug.Log("Player entered portal to: " + _dimension.ToString());
            GameManager.Instance.ChangeDimension(_dimension);
        }
    }
    
    public void ChangeDimension(Dimension d) {
        Debug.Log("Changed dimension to: " + d.ToString());
        _dimension = d;
    }
}
