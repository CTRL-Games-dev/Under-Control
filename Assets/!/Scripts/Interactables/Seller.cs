using UnityEngine;


[RequireComponent(typeof(SimpleInventory))]
public class Seller : MonoBehaviour, IInteractableInventory
{
    [SerializeField] private GameObject _uiPrefab; 
    public SimpleInventory Inventory;
    [SerializeField] private FaceAnimator _faceAnimator;


    void Awake() {
        Inventory = GetComponent<SimpleInventory>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F3)) {
            _faceAnimator.StartAnimation("TALK", 5f); 
        }

    }


    public void Interact() {
        Debug.Log("Interact with seller");
        Player.UICanvas.SetOtherInventory(Inventory.ItemContainer, _uiPrefab, this, "interactable_name_seller_key");
    }

    public void EndInteract() {}
}
