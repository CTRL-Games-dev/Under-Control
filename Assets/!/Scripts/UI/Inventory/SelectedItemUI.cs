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
                _image.sprite = null;
                transform.rotation = Quaternion.identity;
                gameObject.SetActive(false);
            } else {
                transform.rotation = _inventoryItem.Rotated ? Quaternion.Euler(0, 0, 90) : Quaternion.identity; 
                transform.position = Input.mousePosition;

                gameObject.SetActive(true);

                _rectTransform.sizeDelta = new Vector2(InventoryUIManager.TileSize, InventoryUIManager.TileSize);
                _imageRectTransform.sizeDelta = new Vector2(InventoryUIManager.TileSize * _inventoryItem.Size.x, InventoryUIManager.TileSize * _inventoryItem.Size.y);

                _image.sprite = _inventoryItem.Item.Icon;
                if (_inventoryItem.Amount == 1) {
                    _amountText.text = "";
                    return;
                }
                _amountText.text = _inventoryItem.Amount.ToString(); 
            }
        }
    }

    private RectTransform _rectTransform;
    [SerializeField] private RectTransform _imageRectTransform;
    private Image _image;


    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponentInChildren<Image>();
    }

    private void Update()  {
        if(_inventoryItem == null) {
            return;
        }
        transform.position = Input.mousePosition;
    }

    public void Rotate(bool val) {
        if(_inventoryItem == null) {
            return;
        }
        transform.rotation = val ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;
    }
}
