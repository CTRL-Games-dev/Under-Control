using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spl_IceShardSpell", menuName = "Spells/IceShardSpell")]
public class IceShardSpell : Spell {
    public IceShard IceShardPrefab;
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

        IceShard IceShard = Instantiate(
            IceShardPrefab,
            spawnerPosition,
            spellRotation
        );

        IceShard.Initialize(Player.LivingEntity, spellDirection.normalized);
    }
}