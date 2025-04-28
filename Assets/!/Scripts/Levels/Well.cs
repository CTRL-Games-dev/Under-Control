using UnityEngine;

public class Well : MonoBehaviour, IInteractable
{
    bool wasUsed = false;
    public void Interact()
    {
        if(wasUsed) return;
        Player.LivingEntity.Health = Player.LivingEntity.MaxHealth;
        Player.LivingEntity.Mana = Player.LivingEntity.MaxMana;
        Debug.Log("Działa!");
    }
}
