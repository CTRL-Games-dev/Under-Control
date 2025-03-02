using UnityEngine;


[RequireComponent(typeof(SimpleInventory))]

public class Seller : MonoBehaviour
{
    [SerializeField] private GameObject _sellerUIPrefab; 
    public SimpleInventory Inventory;


    void Awake() {
        Inventory = GetComponent<SimpleInventory>();
    }


    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<PlayerController>()) {
            Debug.Log("Player entered seller trigger");
            UICanvas.Instance.SetOtherInventory(Inventory.ItemContainer);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.GetComponent<PlayerController>()) {
            UICanvas.Instance.SetOtherInventory(null);
        }
    }

    public void Interact(PlayerController player)
    {
        Debug.Log("Interacting with seller");

        // _inventoryPanel = Instantiate(_sellerUIPrefab, FindFirstObjectByType<UICanvas>().GetComponent<UICanvas>().InventoryBG.transform);
        // InventoryPanel inventoryPanel = _inventoryPanel.GetComponentInChildren<InventoryPanel>();
        // inventoryPanel.TargetEntityInventory = Inventory;
        // inventoryPanel.RegenerateInventory();
    }
}
