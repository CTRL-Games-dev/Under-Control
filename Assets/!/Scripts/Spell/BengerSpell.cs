using UnityEngine;

public class BengerSpell : Spell {
    public override string Name => "Benger Spell";

    private static readonly Effect bengerEffect = Resources.Load<Effect>("Effects/BengerEffect");

    public override void Cast(LivingEntity caster) {
        caster.ApplyEffect(bengerEffect);

        Debug.Log($"Casting {Name} by {caster.displayName}, effect is: {bengerEffect}");
    }
}