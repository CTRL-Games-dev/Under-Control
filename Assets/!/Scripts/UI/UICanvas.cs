using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [Header("References for children")]
    public GameObject Player;
    public PlayerController PlayerController;
    public LivingEntity PlayerLivingEntity;
    public InventorySystem PlayerInventory;
}
