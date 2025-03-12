using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spl_BengerSpell", menuName = "Spells/BengerSpell")]
public class BengerSpell : Spell {
    public Effect BengerEffect;

    public override void Cast(LivingEntity caster) {
        caster.ApplyEffect(BengerEffect);

        Debug.Log($"Casting {Name} by {caster.DisplayName}, effect is: {BengerEffect}");
    }
}