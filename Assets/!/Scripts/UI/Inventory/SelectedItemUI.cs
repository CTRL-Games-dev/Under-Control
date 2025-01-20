using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _amountText;

    private InventoryItem _inventoryItem = null;
    public InventoryItem InventoryItem { 
        get { return _inventoryItem; } 
        set {
            _inventoryItem = value;
            if (_inventoryItem == null) {
                _image.sprite = null;
                gameObject.SetActive(false);
            } else {
                transform.position = Input.mousePosition;

                gameObject.SetActive(true);

                _rectTransform.sizeDelta = new Vector2(InventoryUIManager.TileSize * _inventoryItem.Size.x, InventoryUIManager.TileSize * _inventoryItem.Size.y);
                _image.sprite = _inventoryItem.ItemData.Icon;
                if (_inventoryItem.Amount == 1) {
                    _amountText.text = "";
                    return;
                }
                _amountText.text = _inventoryItem.Amount.ToString(); 
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
