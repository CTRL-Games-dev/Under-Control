using UnityEngine;


[RequireComponent(typeof(SimpleInventory))]

public class Seller : MonoBehaviour, IInteractableInventory
{
    [SerializeField] private GameObject _uiPrefab; 
    public SimpleInventory Inventory;


    void Awake() {
        Inventory = GetComponent<SimpleInventory>();
    }


    public void Interact(PlayerController player){
        Player.UICanvas.SetOtherInventory(Inventory.ItemContainer, _uiPrefab, this, "SELLER");
    }

    public void EndInteract() {
        
    }
}
