using UnityEngine;
using static InventorySystem;


public class SelectedItemUI : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    private ItemUI _itemUI;
    public ItemUI ItemUI { get { return _itemUI; } }
    private InventoryItem _inventoryItem = null;
    public InventoryItem InventoryItem { 
        get {
            return _inventoryItem;
        } 
        set {
            Debug.Log("Setting inventory item" + value);
            _inventoryItem = value;

            if (_inventoryItem == null) {
                _rectTransform.gameObject.SetActive(false);
                _gridManager.RemoveItem(_itemUI);
            } else {
                _rectTransform.gameObject.SetActive(true);
            }
        }
    }


    private RectTransform _rectTransform;


    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _itemUI = GetComponent<ItemUI>();
    }

    private void Update()  {
        if(_inventoryItem == null) {
            return;
        }
        transform.position = Input.mousePosition;
    }
}
