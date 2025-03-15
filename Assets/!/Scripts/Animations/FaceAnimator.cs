using System;
using System.Collections;
using UnityEngine;

public class FaceAnimator : MonoBehaviour
{
    public enum FaceState {
        Neutral,
        Excited,
        Talk
    }

    [Header("Blinking")]
    [SerializeField] private Material _blinkMaterial;
    [SerializeField] private float _blinkDuration = 0.1f;
    [SerializeField] private float _blinkDelay = 3f;

    // [Header("Neutral")]
    // [SerializeField] private Material[] _neutralMaterials;
    // [SerializeField] private float _neutralSpeed = 0.2f;
    // private int _neutralIndex = 0;

    [Header("Excited")]
    [SerializeField] private Material[] _excitedMaterials;
    [SerializeField] private float _excitedSpeed = 0.1f;
    
    [Header("Talk")]
    [SerializeField] private Material[] _talkMaterials;
    [SerializeField] private float _talkSpeed = 0.1f;

    private Material _originalMaterial;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    public static FaceAnimator Instance;

    public FaceState CurrentFaceState = FaceState.Neutral;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        _originalMaterial = _skinnedMeshRenderer.materials[0];
        InvokeRepeating(nameof(Blink), _blinkDelay, _blinkDelay);
    }

    private void Start() {
        Talk(5);
    }

    public void Blink() {;
        StartCoroutine(blinkCoroutine());
    }

    public void Talk(float duration) {
        StopAllCoroutines();
        CurrentFaceState = FaceState.Talk;
        StartCoroutine(talkCoroutine(duration));
    }

    public void Excited(float duration) {
        StopAllCoroutines();
        CurrentFaceState = FaceState.Excited;
        StartCoroutine(excitedCoroutine(duration));
    }

    private IEnumerator blinkCoroutine() {
        _skinnedMeshRenderer.materials = new Material[] { _blinkMaterial };
        yield return new WaitForSeconds(_blinkDuration);
        _skinnedMeshRenderer.materials = new Material[] {_originalMaterial};   
    }

    private IEnumerator talkCoroutine(float duration) {
        float time = 0;
        int index = 0;
        while (time < duration) {
            _skinnedMeshRenderer.materials = new Material[] { _talkMaterials[index] };
            yield return new WaitForSeconds(_talkSpeed);
            time += _talkSpeed;
            index = (index + 1) % _talkMaterials.Length;
        }
        _skinnedMeshRenderer.materials = new Material[] {_originalMaterial};
        CurrentFaceState = FaceState.Neutral;
    }

    private IEnumerator excitedCoroutine(float duration) {
        float time = 0;
        int index = 0;
        while (time < duration) {
            _skinnedMeshRenderer.materials = new Material[] { _excitedMaterials[index] };
            yield return new WaitForSeconds(_excitedSpeed);
            time += _excitedSpeed;
            index = (index + 1) % _excitedMaterials.Length;
        }
        _skinnedMeshRenderer.materials = new Material[] {_originalMaterial};
        CurrentFaceState = FaceState.Neutral;
    }
}
