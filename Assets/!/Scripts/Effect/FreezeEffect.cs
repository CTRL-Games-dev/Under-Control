using UnityEngine;

[CreateAssetMenu(fileName = "SO_Eff_FreezeEffect", menuName = "Effect/FreezeEffect")]
public class FreezeEffect : Effect {
    public Color FreezeColor;
    public float TintAlphaValue;
    public override void OnApply(LivingEntity entity) {
        entity.TintAnimator.ApplyTint(FreezeColor,TintAlphaValue, Duration);
    }

    public override void OnRemove(LivingEntity entity) {
        entity.TintAnimator.ResetTint();
    }
}