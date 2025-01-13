using UnityEngine;

public class BengerSpell : Spell {
    public override string Name => "Benger Spell";

    private static readonly Effect _bengerEffect = Resources.Load<Effect>("Effects/BengerEffect");

    public override void Cast(LivingEntity caster) {
        caster.ApplyEffect(_bengerEffect);

        Debug.Log($"Casting {Name} by {caster.DisplayName}, effect is: {_bengerEffect}");
    }
}