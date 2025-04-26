using UnityEngine;

public class TintAnimator : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;
    private Material _material;

    private void Awake() {
        if (_meshRenderer == null) return;
        _material = _meshRenderer.materials[0];
    }

    public void SetTint(Color color, float alpha, float duration) {
        if (_meshRenderer == null) return;
        _material.SetColor("_TintColor", color);
        _material.SetFloat("_Alpha", alpha);
        Invoke(nameof(ResetTint), duration);
    }

    public void ResetTint() {
        if (_meshRenderer == null) return;
        _material.SetColor("_TintColor", Color.white);
        _material.SetFloat("_Alpha", 0f);
    }
    
}
