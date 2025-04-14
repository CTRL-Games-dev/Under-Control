using UnityEngine;

public class HitFlashAnimator : MonoBehaviour
{
    [SerializeField] private Material _hitFlashMaterial;
    [SerializeField] private float _flashDuration = 0.1f;
    private SkinnedMeshRenderer _meshRenderer;
    private Material[] _originalMaterials;

    private void Start() {
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>(includeInactive:true);
        _originalMaterials = _meshRenderer.materials;
    }

    public void Flash() {
        _meshRenderer.materials = new Material[] { _hitFlashMaterial };
        Invoke(nameof(resetMaterial), _flashDuration);
    }

    private void resetMaterial() {
        _meshRenderer.materials = _originalMaterials;
    }
}
