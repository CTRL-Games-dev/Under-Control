using UnityEngine;


[RequireComponent(typeof(SimpleInventory))]

public class Seller : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _uiPrefab; 
    public SimpleInventory Inventory;


    void Awake() {
        Inventory = GetComponent<SimpleInventory>();
    }


    // void OnTriggerEnter(Collider other) {
    //     if (other.GetComponent<PlayerController>()) {
    //         UICanvas.Instance.SetOtherInventory(Inventory.ItemContainer, _icon);
    //     }
    // }

    // void OnTriggerExit(Collider other) {
    //     if (other.GetComponent<PlayerController>()) {
    //         UICanvas.Instance.SetOtherInventory(null, null);
    //     }
    // }

    public void Interact(PlayerController player)
    {
        UICanvas.Instance.SetOtherInventory(Inventory.ItemContainer, _uiPrefab);


        // _inventoryPanel = Instantiate(_sellerUIPrefab, FindFirstObjectByType<UICanvas>().GetComponent<UICanvas>().InventoryBG.transform);
        // InventoryPanel inventoryPanel = _inventoryPanel.GetComponentInChildren<InventoryPanel>();
        // inventoryPanel.TargetEntityInventory = Inventory;
        // inventoryPanel.RegenerateInventory();
    }
}
