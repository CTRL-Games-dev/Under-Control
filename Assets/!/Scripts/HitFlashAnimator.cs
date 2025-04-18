using UnityEngine;

public class HitFlashAnimator : MonoBehaviour
{
    [SerializeField] private Material _hitFlashMaterial;
    [SerializeField] private float _flashDuration = 0.1f;
    [SerializeField] private bool _isPlayer;
    private SkinnedMeshRenderer _meshRenderer;
    private Material _originalMaterial;

    private void Awake() {
        if (_isPlayer) return;
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _originalMaterial = _meshRenderer.materials[0];
    }

    public void Flash() {
        if (_isPlayer) return;
        _meshRenderer.materials = new Material[] { _hitFlashMaterial };
        Invoke(nameof(resetMaterial), _flashDuration);
    }

    private void resetMaterial() {
        if (_isPlayer) return;
        _meshRenderer.materials = new Material[] {_originalMaterial};
    }
}
