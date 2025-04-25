using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Eff_FireEffect", menuName = "Effect/FireEffect")]
public class FireEffect : Effect {
    public override void OnApply(LivingEntity entity) {
        Fire fire = entity.GetOrAddComponent<Fire>();
        fire.Stack();
    }

    public override void OnRemove(LivingEntity entity) {
        Fire fire = entity.GetComponent<Fire>();
        fire.Unstack();
    }
}