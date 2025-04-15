using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class BoarChargeAttack : MonoBehaviour {
    public Effect StunEffect;

    void Start() {
        Weapon weapon = GetComponent<Weapon>();

        weapon.OnHit.AddListener(OnHit);
    }

    private void OnHit(LivingEntity victim) {
        victim.ApplyEffect(StunEffect);
    }
}