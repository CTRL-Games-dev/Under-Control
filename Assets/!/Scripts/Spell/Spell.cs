public abstract class Spell {
    public abstract string Name { get; }
    public abstract void Cast(LivingEntity caster);
}