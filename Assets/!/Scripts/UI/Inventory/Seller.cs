using UnityEngine;

public class Seller : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _sellerUIPrefab; 
    [SerializeField] private ItemContainer _inventory;
    public ItemContainer Inventory { get => _inventory; private set => _inventory = value; }
    public GameObject _inventoryPanel;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Player entered seller trigger");
            // _inventoryPanel = Instantiate(_sellerUIPrefab, FindFirstObjectByType<UICanvas>().GetComponent<UICanvas>().InventoryBG.transform);
            // InventoryPanel inventoryPanel = _inventoryPanel.GetComponentInChildren<InventoryPanel>();
            // inventoryPanel.TargetEntityInventory = Inventory;
            // inventoryPanel.RegenerateInventory();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Player exited seller trigger");

        }
        // if (other.CompareTag("Player")) {
        //     Destroy(_inventoryPanel);
        //     EventBus.ItemUIHoverEvent.Invoke(null);
        // }
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
