using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spl_FireballSpell", menuName = "Spells/FireballSpell")]
public class FireballSpell : Spell {
    public Fireball FireballPrefab;

    public override void Cast() {}

    public override void OnCastReady() {
        
        Player.Instance.StartCoroutine(castOnTheEndOfFrame());
    }

    private IEnumerator castOnTheEndOfFrame() {
        yield return new WaitForEndOfFrame();

        Vector3 spawnerPosition = Player.SpellSpawner.transform.position;
        Vector3 spellDirection = Player.Instance.GetMousePosition() - spawnerPosition;
        spellDirection.y = 0;
        
        Quaternion spellRotation = Quaternion.LookRotation(spellDirection);

        Fireball fireball = Instantiate(
            FireballPrefab,
            spawnerPosition,
            spellRotation
        );

        fireball.Initialize(Player.LivingEntity, spellDirection.normalized);
    }
}