using System.Collections;
using DG.Tweening;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Rendering;

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
    private Volume _globalVolume;
    private IEnumerator _currentCoroutine;
    void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    public UnityEngine.Rendering.Volume globalVolume;
    void Start() {
        _globalVolume = FindAnyObjectByType<Volume>();
    }
    void Update() {
        switch(_state) {
            case ControlManagerState.Stopped: break;
            case ControlManagerState.Running: {
                if(_currentCoroutine != null) return;
                if(getEffectChange() > 0) return;
                _currentCoroutine = scheduleNewEffect(15);
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

        if(UnityEngine.Random.Range(0f, 1f) > getEffectChange()) return;

        int effectIndex = UnityEngine.Random.Range(0, 3);
        switch(effectIndex) {
            case 0: _blurEffect(); break;
            case 1: _stunEffect(); break;
            case 2: _invertEffect(); break;
        }

        PlayAnimation();
    }

    private void _blurEffect() {
        Debug.Log("Applying effect: Blur");
        if (globalVolume.profile.TryGet<LensDistortion>(out var lensDistortion))
        {
            float intensity = 0f;
            DOTween.To(() => intensity, x => intensity = x, 0.5f, 0.25f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            DOTween.To(() => intensity, x => intensity = x, -0.5f, 0.5f).SetDelay(0.25f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            DOTween.To(() => intensity, x => intensity = x, 0.5f, 0.5f).SetDelay(0.75f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            DOTween.To(() => intensity, x => intensity = x, -0.5f, 0.5f).SetDelay(1.25f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            DOTween.To(() => intensity, x => intensity = x, 0.5f, 0.5f).SetDelay(1.75f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            DOTween.To(() => intensity, x => intensity = x, -0.5f, 0.5f).SetDelay(2.25f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            DOTween.To(() => intensity, x => intensity = x, 0.5f, 0.5f).SetDelay(2.75f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            DOTween.To(() => intensity, x => intensity = x, -0.5f, 0.5f).SetDelay(3.25f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            DOTween.To(() => intensity, x => intensity = x, 0.5f, 0.5f).SetDelay(3.75f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            DOTween.To(() => intensity, x => intensity = x, -0.5f, 0.5f).SetDelay(4.25f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            DOTween.To(() => intensity, x => intensity = x, 0f, 0.5f).SetDelay(4.75f).OnUpdate(() => {
                lensDistortion.intensity.Override(intensity);
            });
            
        }
    }

    private void _invertEffect() {
        Debug.Log("Applying new Screen Invert");
        Player.Instance.InvertedControls = true;
        StartCoroutine(_invertWait(5f));
    }

    private IEnumerator _invertWait(float secs) {
        yield return new WaitForSeconds(secs);
        Player.Instance.InvertedControls = false;
    }

    private void _stunEffect() {
        Debug.Log("Applying new effect Stun");
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
