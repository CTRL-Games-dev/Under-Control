using System;
using System.Collections.Generic;
using UnityEngine;

public class FaceAnimator : MonoBehaviour
{
    [Serializable]
    public struct FaceAnimation {
        public string AnimationName;
        public List<Vector2Int> Positions;
        public float Speed;
        public bool IsRandomized;
        public bool CanBlink;
    }

    [Header("References")]
    [SerializeField] private Renderer _faceRenderer;
    [SerializeField] private int _sheetSize = 5;

    [Space(1)]
    [SerializeField] private FaceAnimation _defaultAnimation;
    [Space(1)]
    [SerializeField] private Vector2Int _blinkPosition;
    [SerializeField] private float _blinkInterval = 0.5f, _blinkSpeed = 0.1f;

    [Header("Custom Animations")]
    [SerializeField] private List<FaceAnimation> _customAnimations = new List<FaceAnimation>();

    private float _textureOffset;

    private float _animationTimer = 0;
    private float _blinkTimer = 0;
    private float _customDurationTimer = 0;
    private float _customGoalDuration = 0;

    private FaceAnimation _currentFaceAnimation;
    private int _animationIndex = 0;


    private void Awake() {
        _currentFaceAnimation = _defaultAnimation;
        _textureOffset = 1f / _sheetSize;
    }

    public void FixedUpdate() {
        _animationTimer += Time.deltaTime;
        _customDurationTimer += Time.deltaTime;
        _blinkTimer += Time.deltaTime;


        if (_blinkTimer >= _blinkInterval && _currentFaceAnimation.CanBlink) {
            if (_blinkTimer <= _blinkInterval + _blinkSpeed) {
                _faceRenderer.material.mainTextureOffset = getRealOffset(_blinkPosition);
                return;
            } else {
                _blinkTimer = 0;
            }
        }

        if (_animationTimer >= _currentFaceAnimation.Speed) {
            _animationTimer = 0;
            _faceRenderer.material.mainTextureOffset = getRealOffset(_currentFaceAnimation.Positions[_animationIndex]);
            if (_currentFaceAnimation.IsRandomized) {
                _animationIndex = UnityEngine.Random.Range(0, _currentFaceAnimation.Positions.Count);
            } else {
                _animationIndex = (_animationIndex + 1) % _currentFaceAnimation.Positions.Count;
            }
        }

        if (_customGoalDuration <= 0) return;
        if (_customDurationTimer >= _customGoalDuration) {
            _currentFaceAnimation = _defaultAnimation;
            _animationIndex = 0;
            _customGoalDuration = 0;
        }
    }

    // Infinite by default
    public void StartInfiniteAnimation(string animationName) {
        StartAnimation(animationName, -1);
    }

    public void StartAnimation(string animationName, float duration) {
        foreach (var animation in _customAnimations) {
            if (animation.AnimationName == animationName) {
                _currentFaceAnimation = animation;
                _animationIndex = 0;
                _customGoalDuration = duration;
                _customDurationTimer = 0;
                _blinkTimer = 0;
                return;
            }
        }

        Debug.LogWarning($"Animation {animationName} not found!");
    }

    public void EndAnimation() {
        _currentFaceAnimation = _defaultAnimation;
        _animationIndex = 0;
        _customGoalDuration = 0;
        _customDurationTimer = 0;
        _blinkTimer = 0;
    }

    private Vector2 getRealOffset(Vector2Int offset) {
        return new Vector2(offset.x * _textureOffset, -offset.y * _textureOffset - _textureOffset);
    }
}
