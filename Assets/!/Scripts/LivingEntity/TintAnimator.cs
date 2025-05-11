using System.Collections.Generic;
using UnityEngine;

public class TintAnimator : MonoBehaviour
{
    [SerializeField] private List<SkinnedMeshRenderer> _meshRenderers;
    [SerializeField] private Color _hitTintColor = Color.red;
    private List<Material> _tintMaterials = new();
    private List<Material> _hitMaterials = new();

    private void Start() {
        if (_meshRenderers.Count == 0) return;
        foreach (SkinnedMeshRenderer s in _meshRenderers) {
            if (s == null) continue;
            _tintMaterials.Add(s.materials[0]);
            _hitMaterials.Add(s.materials[1]);
        }
    }

    public void ApplyTint(Color color, float alpha, float duration) {
        SetTint(color, alpha);
        Invoke(nameof(ResetTint), duration);
    }

    public void SetTint(Color color, float alpha) {
        if (_meshRenderers.Count == 0) return;
        //Debug.Log(_materials.Count);

        foreach (Material m in _tintMaterials) {
            if (m == null) continue;
            //Debug.Log(m);
            m.SetColor("_TintColor", color);
            m.SetFloat("_Alpha", alpha);
        }
    }

    public void ResetTint() {
        foreach (Material m in _tintMaterials) {
            if (m == null) continue;
            m.SetColor("_TintColor", Color.white);
            m.SetFloat("_Alpha", 0f);
        }
    }

    public void HitTint() {
        if (_meshRenderers.Count == 0) return;
        resetHitTint();

        foreach (Material m in _hitMaterials) {
            if (m == null) continue;
            m.SetColor("_TintColor", Color.red);
            m.SetFloat("_Alpha", 1f);
        }

        Invoke(nameof(resetHitTint), 0.1f);
    }

    private void resetHitTint() {
        foreach (Material m in _hitMaterials) {
            if (m == null) continue;
            m.SetColor("_TintColor", Color.white);
            m.SetFloat("_Alpha", 0f);
        }
    }
}
