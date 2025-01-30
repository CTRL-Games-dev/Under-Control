using TMPro;
using UnityEngine;

public class ItemInfoPanel : MonoBehaviour
{
    [Header("Item Info Panel")]
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemDescription;

    private RectTransform _rectTransform;
    private UICanvas _uiCanvasParent;




    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _uiCanvasParent = gameObject.GetComponentInParent<UICanvas>();
    }
    
    private void Update() {
        transform.position = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _rectTransform.rect.width), 
            Mathf.Clamp(Input.mousePosition.y, _rectTransform.rect.height, Screen.height)
        );
    }

    public void ShowItemInfo(InventoryItem item) {
        gameObject.SetActive(item != null);
        transform.position = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _rectTransform.rect.width), 
            Mathf.Clamp(Input.mousePosition.y, _rectTransform.rect.height, Screen.height)
        );
        if (item == null) return;
        _itemName.text = item.ItemData.DisplayName;
        _itemDescription.text = item.ItemData.Description;
    }
}
