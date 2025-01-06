using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public float regenRate = 1f;

    // Cooldown after hit for regen
    public Cooldown regenCooldown = new Cooldown(2);

    public UnityEvent OnDeathEvent;
    public UnityEvent OnDamageTakenEvent;

    void Update() {
        // Regen
        if(regenCooldown.IsReady()) {
            health += regenRate * Time.deltaTime;
            if(health > maxHealth) {
                health = maxHealth;
            }
        }        
    }

    public void TakeDamage(float damage)
    {
        regenCooldown.Execute();

        if(health == 0) {
            return;
        }

        health -= damage;
        if (health <= 0)
        {
            health = 0;
            OnDeathEvent.Invoke();
        }
    }
}