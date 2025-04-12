using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spl_IceShardSpell", menuName = "Spells/IceShardSpell")]
public class IceshardSpell : Spell {
    public IceShard IceShardPrefab;

    public override void Cast() {
        IceShard shard = Instantiate(
            IceShardPrefab,
            Player.Instance.transform.position + Vector3.up,
            Player.Instance.transform.rotation
        );
        
        // TODO

        shard.Initialize(Player.LivingEntity, Player.Instance.GetMousePosition() - Player.Instance.transform.position);
    }
}