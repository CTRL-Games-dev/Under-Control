using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [SerializeField] private Dimension _dimension = Dimension.HUB;
    public UnityEvent<Dimension> PlayerEnteredPortal;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered portal to: " + _dimension.ToString());
        GameManager.Instance.ChangeDimension(_dimension);
    }
    
    public void ChangeDimension(Dimension d) {
        Debug.Log("Changed dimension to: " + d.ToString());
        _dimension = d;
    }

    public void SetDimension(Dimension d)
    {
        _dimension = d;
    }
    public void SetDimensionAndActivate(Dimension d)
    {
        SetDimension(d);
        gameObject.SetActive(true);
    }
    public void EnablePortal(bool enable)
    {
        Debug.Log($"Portal enabled: {enable}");
        gameObject.SetActive(enable);
    }
}
