using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spl_FreezeSpell", menuName = "Spells/FreezeSpell")]
public class IceshardSpell : Spell {
    public IceShard IceShardPrefab;

    public override void Cast() {
        FreezeBall ball = Instantiate(
            IceShardPrefab,
            Player.Instance.transform.position + Vector3.up,
            Player.Instance.transform.rotation
        );
        
        ball.Initialize(Player.LivingEntity, Player.Instance.GetMousePosition() - Player.Instance.transform.position);
    }
}