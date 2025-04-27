using System.Collections.Generic;
using UnityEngine;

public class TintAnimator : MonoBehaviour
{
    [SerializeField] private List<SkinnedMeshRenderer> _meshRenderers;
    private List<Material> _materials = new();

    private void Start() {
        if (_meshRenderers.Count == 0) return;
        foreach (SkinnedMeshRenderer s in _meshRenderers) {
            if (s == null) continue;
            Debug.Log(s.materials[0]);
            _materials.Add(s.materials[0]);
        }
    }

    public void SetTint(Color color, float alpha, float duration) {
        if (_meshRenderers.Count == 0) return;
        Debug.Log(_materials.Count);

        foreach (Material m in _materials) {
            if (m == null) continue;
            Debug.Log(m);
            m.SetColor("_TintColor", color);
            m.SetFloat("_Alpha", alpha);
        }

        Invoke(nameof(ResetTint), duration);
    }

    public void ResetTint() {
        foreach (Material m in _materials) {
            if (m == null) continue;
            m.SetColor("_TintColor", Color.white);
            m.SetFloat("_Alpha", 0f);
        }
    }
    
}
