using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EffectUI : MonoBehaviour
{
    public LivingEntity.EffectData EffectData;
    public float LocalX;

    [SerializeField] private Image _fillImage, _icon;
    private float _duration = 0f;
    private bool _isSetup = false;

    private bool _isHovered = false;

    private void Update() {
        if (!_isSetup) return;
        
        if (!float.IsInfinity(EffectData.Effect.Duration)) {
            _duration -= Time.deltaTime;
            _fillImage.fillAmount = _duration / EffectData.Effect.Duration;

            if (_duration <= 0) {
                if (_isHovered) Player.UICanvas.HUDCanvas.HideMoreInfo();
                Player.UICanvas.HUDCanvas.RemoveEffectUI(this);
                Destroy(gameObject);
            } 
        } 

        if (_isHovered) {
            Player.UICanvas.HUDCanvas.SetDuationText(_duration);
        }
    }


    public void Setup(LivingEntity.EffectData effectData) {
        EffectData = effectData;
        _duration = EffectData.Effect.Duration;
        _icon.sprite = EffectData.Effect.Icon;
        _fillImage.fillAmount = float.IsInfinity(EffectData.Effect.Duration) ? 0 : 1;
        _isSetup = true;
    }

    public void OnPointerEnter() {
        _isHovered = true;
        Player.UICanvas.HUDCanvas.ShowMoreInfo(this);
    }

    public void OnPointerExit() {
        _isHovered = false;
        Player.UICanvas.HUDCanvas.HideMoreInfo();
    }
}
