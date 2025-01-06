using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public float regenRate = 1f;

    public UnityEvent OnDeathEvent;
    public UnityEvent OnDamageTakenEvent;

    public void TakeDamage(float damage)
    {
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