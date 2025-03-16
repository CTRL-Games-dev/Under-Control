using UnityEngine;

public class HitFlashAnimator : MonoBehaviour
{
    [SerializeField] private Material _hitFlashMaterial;
    [SerializeField] private float _flashDuration = 0.1f;
    private SkinnedMeshRenderer _meshRenderer;
    private Material _originalMaterial;

    private void Awake() {
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _originalMaterial = _meshRenderer.materials[0];
    }

    public void Flash() {
        _meshRenderer.materials = new Material[] { _hitFlashMaterial };
        Invoke(nameof(resetMaterial), _flashDuration);
    }

    private void resetMaterial() {
        _meshRenderer.materials = new Material[] {_originalMaterial};
    }
}
