using UnityEngine;

public class TestSpell : Spell {
    public override string Name => "Test Spell";

    private static readonly Effect _testEffect = Resources.Load<Effect>("Effects/TestEffect");

    public override void Cast(LivingEntity caster) {
        caster.ApplyEffect(_testEffect);

        Debug.Log($"Casting {Name} by {caster.DisplayName}, effect is: {_testEffect}");
    }
}