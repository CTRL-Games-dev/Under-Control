using UnityEngine;

public abstract class Spell : ScriptableObject {
    public string Name;
    public string Description;
    public Sprite Icon;
    public float Mana;
    public float CooldownTime;

    public abstract void Cast(LivingEntity caster);
}