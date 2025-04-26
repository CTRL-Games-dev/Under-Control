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
    [SerializeField] private Renderer[] _portalInsideRenderers;
    [HideInInspector] public float Influence;

    private void Awake() {
        _portalInsideRenderer = _portalInside.GetComponent<Renderer>();        
    }

    void Start() {
        setInfluence();
    }

    protected void FixedUpdate() {
        if (_portalInsideRenderers.Length == 0) return;

        float xOffset = Mathf.Sin(Time.time) * 0.2f;
        float yOffset = Mathf.Cos(Time.time) * 0.2f;
        
        foreach (Renderer r in _portalInsideRenderers) {
            r.material.mainTextureOffset = new Vector2(xOffset, yOffset);
        }
    }

    protected abstract void setInfluence();

    private Renderer _portalInsideRenderer;


    void OnTriggerEnter(Collider other) {
        Debug.Log("Player entered portal to: " + _dimension.ToString());
        GetComponent<Collider>().enabled = false;
        GameManager.Instance.ChangeDimension(_dimension, Influence);
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
