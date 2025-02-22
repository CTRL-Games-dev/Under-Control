using UnityEngine;

public class Seller : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _sellerUIPrefab; 
    [SerializeField] private EntityInventory _inventory = new EntityInventory();
    public EntityInventory Inventory { get => _inventory; private set => _inventory = value; }
    public GameObject _inventoryPanel;

    // void OnTriggerEnter(Collider other) {
    //     if (other.CompareTag("Player")) {
    //         _inventoryPanel = Instantiate(_sellerUIPrefab, FindFirstObjectByType<UICanvas>().GetComponent<UICanvas>().InventoryBG.transform);
    //         InventoryPanel inventoryPanel = _inventoryPanel.GetComponentInChildren<InventoryPanel>();
    //         inventoryPanel.TargetEntityInventory = Inventory;
    //         inventoryPanel.RegenerateInventory();
    //     }
    // }

    // void OnTriggerExit(Collider other) {
    //     if (other.CompareTag("Player")) {
    //         Destroy(_inventoryPanel);
    //         EventBus.ItemUIHoverEvent.Invoke(null);
    //     }
    // }

    public void Interact(PlayerController player)
    {
        Debug.Log("Interacting with seller");

        // _inventoryPanel = Instantiate(_sellerUIPrefab, FindFirstObjectByType<UICanvas>().GetComponent<UICanvas>().InventoryBG.transform);
        // InventoryPanel inventoryPanel = _inventoryPanel.GetComponentInChildren<InventoryPanel>();
        // inventoryPanel.TargetEntityInventory = Inventory;
        // inventoryPanel.RegenerateInventory();
    }
}
