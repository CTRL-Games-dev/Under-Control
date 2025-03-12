using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spl_TestSpell", menuName = "Spells/TestSpell")]
public class TestSpell : Spell {
    public Effect TestEffect;

    public override void Cast(LivingEntity caster) {
        caster.ApplyEffect(TestEffect);

        Debug.Log($"Casting {Name} by {caster.DisplayName}, effect is: {TestEffect}");
    }
}