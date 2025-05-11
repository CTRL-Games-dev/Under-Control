using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Eff_FreezeEffect", menuName = "Effect/FreezeEffect")]
public class FreezeEffect : Effect {
    public override void OnApply(LivingEntity entity) {
        Freeze freeze = entity.GetOrAddComponent<Freeze>();
        freeze.Stack();
    }

    public override void OnRemove(LivingEntity entity) {
        Freeze freeze = entity.GetComponent<Freeze>();
        freeze.Unstack();
    }
}