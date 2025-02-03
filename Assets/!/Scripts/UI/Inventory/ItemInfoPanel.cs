using TMPro;
using UnityEngine;

public class ItemInfoPanel : MonoBehaviour
{
    [Header("Item Info Panel")]
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemDescription;
    [SerializeField] private TextMeshProUGUI _itemValue;
    [SerializeField] private TextMeshProUGUI _itemAmount;

    private RectTransform _rectTransform;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    private void Update() {
        transform.position = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _rectTransform.rect.width), 
            Mathf.Clamp(Input.mousePosition.y, _rectTransform.rect.height, Screen.height)
        );
    }

    public void ShowItemInfo(ItemUI itemUI) {
        gameObject.SetActive(itemUI != null);
        if (itemUI == null) return;
        InventoryItem item = itemUI.InventoryItem;
        transform.position = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _rectTransform.rect.width), 
            Mathf.Clamp(Input.mousePosition.y, _rectTransform.rect.height, Screen.height)
        );
        _itemName.text = item.ItemData.DisplayName;
        _itemDescription.text = item.ItemData.Description;
        int value = itemUI.CurrentInventoryPanel.IsSellerInventory ? item.ItemData.Value : item.ItemData.Value / 2;
        _itemValue.text = value + " (" + value * item.Amount + ")";
        _itemAmount.text = '×' + item.Amount.ToString();
    }
}
