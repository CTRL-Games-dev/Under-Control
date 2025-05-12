using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UI;

public class VektharControlManager : MonoBehaviour
{
    enum ControlManagerState {
        Running,
        Stopped,
    }
    [Header("UI")]
    [SerializeField] private Image _eyesPanel;
    [SerializeField] private RectTransform _eyesPanelTransform;
    [Header("Effects")]
    public StunEffect StunEffectData;
    public static VektharControlManager Instance; 
    [SerializeField] private ControlManagerState _state;
    private IEnumerator _currentCoroutine;
    void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    void Update() {
        switch(_state) {
            case ControlManagerState.Stopped: break;
            case ControlManagerState.Running: {
                if(_currentCoroutine != null) return;
                // if(getEffectChange() > 0) return;
                _currentCoroutine = scheduleNewEffect(3);
                StartCoroutine(_currentCoroutine);
            } break;
        }
    }
    private float getEffectChange() {
        float influence = GameManager.Instance.TotalInfluence;
        float chance = (influence-30) / (100 - 30);
        return chance;
    }

    private IEnumerator scheduleNewEffect(float secs) {
        yield return new WaitForSeconds(secs);
        doRandomEffect();
    }
    private void doRandomEffect() {
        // Do funny effect
        Debug.Log("Applying new effect");
        _currentCoroutine = null;

        //if(UnityEngine.Random.Range(0f, 1f) > getEffectChange()) return;

        int effectIndex = UnityEngine.Random.Range(0, 3);
        switch(effectIndex) {
            case 0: _blurEffect(); break;
            case 1: _screenInvertEffect(); break;
            case 2: _stunEffect(); break;
        }

        PlayAnimation();
    }

    private void _blurEffect() {
        Player.LivingEntity.ApplyEffect(StunEffectData);
    }

    private void _screenInvertEffect() {
        Player.LivingEntity.ApplyEffect(StunEffectData);
    }

    private void _stunEffect() {
        Player.LivingEntity.ApplyEffect(StunEffectData);
    }

    public void StartControlManager() {
        Debug.Log("Starting Control Manager");
        _state = ControlManagerState.Running;
    }

    public void StopControlManager() {
        Debug.Log("Stopping Control Manager");
        if (_currentCoroutine != null) {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    public void PlayAnimation() {
        Vector3 scale = _eyesPanel.transform.localScale;
        _eyesPanel.DOKill();
        _eyesPanel.DOFade(1, 0.3f);
        _eyesPanelTransform.DOKill();
        _eyesPanelTransform.DOScale(scale * 1.0f, 1f);
        _eyesPanel.DOFade(0, 0.3f).SetDelay(0.7f).OnComplete(() => {
            _eyesPanel.transform.localScale = scale;
        });
    }
}
