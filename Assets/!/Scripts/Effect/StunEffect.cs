using UnityEngine;

[CreateAssetMenu(fileName = "SO_Eff_StunEffect", menuName = "Effect/StunEffect")]
public class StunEffect : Effect {
    public override void Apply(LivingEntity victim) {
        victim.OnStunned.Invoke(Duration);
    }
}