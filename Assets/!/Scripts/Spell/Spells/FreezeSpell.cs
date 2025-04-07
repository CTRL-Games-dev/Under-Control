using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spl_FreezeSpell", menuName = "Spells/FreezeSpell")]
public class FreezeSpell : Spell {
    public FreezeBall BallPrefab;

    public override void Cast(LivingEntity caster) {
        FreezeBall ball = Instantiate(BallPrefab, caster.transform.position + Vector3.up, caster.transform.rotation);
        ball.Initialize(caster, caster.transform.rotation * Vector3.forward);
    }
}