using UnityEngine;

[CreateAssetMenu(fileName = "SO_Spl_FreezeSpell", menuName = "Spells/FreezeSpell")]
public class FreezeSpell : Spell {
    public FreezeBall BallPrefab;

    public override void Cast() {
        FreezeBall ball = Instantiate(
            BallPrefab,
            Player.Instance.transform.position + Vector3.up,
            Player.Instance.transform.rotation
        );
        
        ball.Initialize(Player.LivingEntity, Player.Instance.GetMousePosition() - Player.Instance.transform.position);
    }
}