using UnityEngine;

public class TestSpell : Spell {
    public override string Name => "Test Spell";

    private static readonly Effect testEffect = Resources.Load<Effect>("Effects/TestEffect");

    public override void Cast(LivingEntity caster) {
        caster.ApplyEffect(testEffect);

        Debug.Log($"Casting {Name} by {caster.displayName}, effect is: {testEffect}");
    }
}