using System.Collections.Generic;
using UnityEngine;

public class FaceAnimator : MonoBehaviour
{
    public enum FaceState {
        Neutral,
        Excited,
        Talk
    }

    [SerializeField] private Renderer _faceRenderer;
    [Header("Blink")]
    [SerializeField] private Vector2 _blinkSprite;
    [SerializeField] private float _blinkInterval = 0.5f, _blinkSpeed = 0.1f;
    
    [Header("Talk")]
    [SerializeField] private Vector2[] _talkSprites;
    [SerializeField] private float _talkSpeed = 0.1f;

    [Header("Excited")]
    [SerializeField] private Vector2[] _excitedSprites;
    [SerializeField] private float _excitedSpeed = 0.1f;

    [Header("Neutral")]
    [SerializeField] private Vector2[] _neutralSprites;
    [SerializeField] private float _neutralSpeed = 0.1f;

    public static FaceAnimator Instance;

    public FaceState CurrentFaceState = FaceState.Neutral;

    private float _animationTimer = 0;
    private float _blinkTimer = 0;
    private float _durationTimer = 0;
    private float _goalDuration = 0;
    private int _index = 0;

    private Dictionary<FaceState, FaceAnimation> _faceSprites;
    private Vector2[] _currentFaceSprites => _faceSprites[CurrentFaceState].Sprites;

    private struct FaceAnimation {
        public Vector2[] Sprites;
        public float Speed;
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }

        _faceSprites = new Dictionary<FaceState, FaceAnimation> {
            { FaceState.Neutral, new FaceAnimation { Sprites = _neutralSprites, Speed = _neutralSpeed } },
            { FaceState.Excited, new FaceAnimation { Sprites = _excitedSprites, Speed = _excitedSpeed } },
            { FaceState.Talk, new FaceAnimation { Sprites = _talkSprites, Speed = _talkSpeed } }
        };
    }

    public void FixedUpdate() {
        _animationTimer += Time.deltaTime;
        _blinkTimer += Time.deltaTime;
        _durationTimer += Time.deltaTime;

        if (_blinkTimer >= _blinkInterval && CurrentFaceState == FaceState.Neutral) {
            _faceRenderer.material.mainTextureOffset = getRealOffset(_blinkSprite);
            Invoke(nameof(resetTime), _blinkSpeed);
            return;
        }

        if (_animationTimer >= _faceSprites[CurrentFaceState].Speed) {
            _animationTimer = 0;
            _faceRenderer.material.mainTextureOffset = getRealOffset(_currentFaceSprites[_index]);
            _index = (_index + 1) % _currentFaceSprites.Length;
        }

        if (_goalDuration <= 0) return;
        if (_durationTimer >= _goalDuration) {
            CurrentFaceState = FaceState.Neutral;
            _index = 0;
            _goalDuration = 0;
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            Talk(3);
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            Excited(1);
        }
    }

    public void Talk(float duration) {
        CurrentFaceState = FaceState.Talk;
        _index = 0;
        _goalDuration = duration;
        _durationTimer = 0;
    }

    public void Excited(float duration) {
        CurrentFaceState = FaceState.Excited;
        _index = 0;
        _goalDuration = duration;
        _durationTimer = 0;
    }

    private void resetTime() {
        _blinkTimer = 0;
    }

    private Vector2 getRealOffset(Vector2 offset) {
        return new Vector2(offset.x * 0.2f, -offset.y * 0.2f - 0.2f);
    }
}
