using TMPro;
using UnityEngine;

public class ItemInfoPanel : MonoBehaviour
{
    [Header("Item Info Panel")]
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemDescription;

    private RectTransform _rectTransform;

    private InventoryItem _inventoryItem;
    public InventoryItem InventoryItem {set {
        gameObject.SetActive(value != null);
        _inventoryItem = value;
        if (_inventoryItem == null) {
            return;
        }
        _itemName.text = _inventoryItem.ItemData.DisplayName;
        _itemDescription.text = _inventoryItem.ItemData.Description;
    } get {
        return _inventoryItem;
        } 
    }

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    private void Update() {
        if (InventoryItem == null) {
            return;
        }
        transform.position = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _rectTransform.rect.width), 
            Mathf.Clamp(Input.mousePosition.y, _rectTransform.rect.height, Screen.height)
        );
    }
}
