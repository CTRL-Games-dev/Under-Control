using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public float regenRate = 1;
    public float timeToRegenAfterDamage = 2;

    private float lastDamageTime = 0;

    public UnityEvent OnDeath;
    public UnityEvent<DamageTakenEventData> OnDamageTaken;

    void Update() {
        // Regen
        if(Time.time - lastDamageTime > timeToRegenAfterDamage) {
            health += regenRate * Time.deltaTime;
            if(health > maxHealth) {
                health = maxHealth;
            }
        }        
    }

    public void TakeDamage(Damage damage)
    {
        lastDamageTime = Time.time;

        // Check if entity is dead
        if(health == 0) {
            return;
        }

        float desiredDamageAmount = damage.value;
        // TODO: Calculate damage based on damage type, current entity modifiers, spells and what not

        float actualDamageAmount = desiredDamageAmount;
        if(actualDamageAmount > health) {
            actualDamageAmount = health;
        }

        health -= actualDamageAmount;

        OnDamageTaken.Invoke(new DamageTakenEventData {
            damage = damage,
            desiredDamageAmount = desiredDamageAmount,
            actualDamageAmount = actualDamageAmount
        });

        if (health == 0)
        {
            OnDeath.Invoke();
        }
    }
}