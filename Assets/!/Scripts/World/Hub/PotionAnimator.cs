using UnityEngine;

public class PotionAnimator : MonoBehaviour
{
    [SerializeField] private MeshRenderer _potionMeshRenderer;
    [SerializeField] private int _materialIndex = 2;
    [SerializeField] private float _changeSpeed = 0.1f;
    [SerializeField] private Color _emissionColor;

    private Material _potionMaterial;

    private void Awake() {
        _potionMaterial = _potionMeshRenderer.materials[_materialIndex];
        _potionMaterial.SetColor("_EmissionColor", _emissionColor);
    }


    void Update() {
        float intensity = Mathf.PingPong(Time.time, _changeSpeed);
        _potionMaterial.SetFloat("_Intensity", intensity);
        
    }
}
