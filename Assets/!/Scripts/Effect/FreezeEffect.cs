using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Eff_FreezeEffect", menuName = "Effect/FreezeEffect")]
public class FreezeEffect : Effect {
    public Color FreezeColor;
    public float TintAlphaValue;
    public override void OnApply(LivingEntity entity) {
        entity.TintAnimator.ApplyTint(FreezeColor,TintAlphaValue, Duration);
        Debug.Log("snigger");
        //TODO LONDEK DODAJ LOCK ROTATION :3
    }

    public override void OnRemove(LivingEntity entity) {
        entity.TintAnimator.ResetTint();
    }
}