using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventorySystem;


public class SelectedItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _amountText;

    private InventoryItem _inventoryItem = null;
    public InventoryItem InventoryItem { 
        get { return _inventoryItem; } 
        set {
            _inventoryItem = value;
            if (_inventoryItem == null) {
                gameObject.SetActive(false);
            } else {
                gameObject.SetActive(true);

                _rectTransform.sizeDelta = new Vector2(InventoryUIManager.TileSize * _inventoryItem.Size.x, InventoryUIManager.TileSize * _inventoryItem.Size.y);
                _image.sprite = _inventoryItem.item.Icon;
                if (_inventoryItem.amount == 1) {
                    _amountText.text = "";
                    return;
                }
                _amountText.text = _inventoryItem.amount.ToString(); 
            }
        }
    }

    private RectTransform _rectTransform;
    private Image _image;


    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    private void Update()  {
        if(_inventoryItem == null) {
            return;
        }
        transform.position = Input.mousePosition;
    }
}
