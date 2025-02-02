using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Seller : MonoBehaviour
{
    [SerializeField] private GameObject _sellerUIPrefab; 
    [SerializeField] private EntityInventory _inventory = new EntityInventory();
    public EntityInventory Inventory { get => _inventory; private set => _inventory = value; }
    public GameObject _inventoryPanel;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            _inventoryPanel = Instantiate(_sellerUIPrefab, FindFirstObjectByType<UICanvas>().GetComponent<UICanvas>().InventoryBG.transform);
            InventoryPanel inventoryPanel = _inventoryPanel.GetComponentInChildren<InventoryPanel>();
            inventoryPanel.TargetEntityInventory = Inventory;
            inventoryPanel.RegenerateInventory();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            Destroy(_inventoryPanel);
            EventBus.ItemUIHoverEvent.Invoke(null);
        }
    }
}
