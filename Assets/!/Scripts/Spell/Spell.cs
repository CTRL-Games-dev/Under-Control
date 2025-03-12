using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spl_Spell", menuName = "Spell")]
public abstract class Spell : ScriptableObject {
    public string Name;
    public string Description;
    public Sprite Icon;

    public abstract void Cast(LivingEntity caster);
}