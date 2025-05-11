using System.Collections.Generic;
using UnityEngine;

public class TintAnimator : MonoBehaviour
{
    public class Tint {
        public Color Color { get; private set; }
        public float Alpha { get; private set; }
        public float Deadline { get; private set; }
        public bool IsActive { get; private set; }

        private TintAnimator _owner;

        public Tint(TintAnimator owner, Color color, float alpha, float deadline) {
            _owner = owner;
            Color = color;
            Alpha = alpha;
            Deadline = deadline;
            IsActive = true;
        }

        public void Stop() {
            if(!IsActive) return;

            _owner.removeTint(this);
            IsActive = false;
        }
    }

    [SerializeField] private List<SkinnedMeshRenderer> _meshRenderers;
    [SerializeField] private Color _hitTintColor = Color.red;
    private List<Material> _tintMaterials = new();
    private List<Material> _hitMaterials = new();

    private List<Tint> _tints = new();

    private void Start() {
        if (_meshRenderers.Count == 0) return;

        foreach (SkinnedMeshRenderer s in _meshRenderers) {
            if (s == null) continue;
            _tintMaterials.Add(s.materials[0]);
            _hitMaterials.Add(s.materials[1]);
        }
    }

    private void Update() {
        List<Tint> tintsClone = new List<Tint>(_tints);

        foreach (Tint tint in tintsClone) {
            if (tint.Deadline < Time.time) tint.Stop();
        }
    }

    public Tint ApplyTint(Color color, float alpha, float duration) {
        Tint newTint = new Tint(this, color, alpha, Time.time +duration);

        _tints.Add(newTint);

        updateTint();

        return newTint;
    }

    private void updateTint() {
        if (_tints.Count == 0) {
            resetTint();
            return;
        }

        Tint activeTint = _tints[_tints.Count - 1];
        setTint(activeTint);
    }

    private void resetTint() {
        if (_meshRenderers.Count == 0) return;

        foreach (Material m in _tintMaterials) {
            if (m == null) continue;
            m.SetColor("_TintColor", Color.white);
            m.SetFloat("_Alpha", 0f);
        }
    }

    private void setTint(Tint tint) {
        if (_meshRenderers.Count == 0) return;

        foreach (Material m in _tintMaterials) {
            if (m == null) continue;

            m.SetColor("_TintColor", tint.Color);
            m.SetFloat("_Alpha", tint.Alpha);
        }
    }

    public void HitTint() {
        ApplyTint(_hitTintColor, 1, 0.1f);
    }

    protected void removeTint(Tint tint) {
        _tints.Remove(tint);
        updateTint();
    }
}
