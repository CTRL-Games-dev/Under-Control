using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]

public abstract class Portal : MonoBehaviour
{
    [SerializeField] private Dimension _dimension = Dimension.HUB;
    [SerializeField] private GameObject _portalInside;
    [SerializeField] private BoxCollider _collider;
    public UnityEvent<Dimension> PlayerEnteredPortal;
    [HideInInspector] public float Influence;
    protected abstract void setInfluence();

    private Renderer _portalInsideRenderer;

    void Start() {
        setInfluence();
        _portalInsideRenderer = _portalInside.GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("Player entered portal to: " + _dimension.ToString());
        GameManager.Instance.ChangeDimension(_dimension, Influence);
    }
    
    public void ChangeDimension(Dimension d) {
        Debug.Log("Changed dimension to: " + d.ToString());
        _dimension = d;
    }

    public void SetDimension(Dimension d) {
        _dimension = d;
    }

    public void SetDimensionAndActivate(Dimension d) {
        _portalInside.SetActive(true);
        _collider.enabled = true;
        SetDimension(d);
        gameObject.SetActive(true);
    }

    public void EnablePortal(bool enable) {
        Debug.Log($"Portal enabled: {enable}");
        gameObject.SetActive(enable);
    }

    public void FadePortalColor(Color color, float speed) {
        _portalInsideRenderer.material.DOColor(color, speed);
    }
}
