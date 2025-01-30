using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private RectTransform _containerRectTransform;
    [SerializeField] private RectTransform _pivotRectTransform;
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private GameObject _quantityGameObject;

    private Image _image;
    public Image Image { get => _image; }

    private TextMeshProUGUI _amountText;
    public int Amount { 
        set {
            if (value == 1) {
                _amountText.text = "";
                return;
            }
            _amountText.text = value.ToString(); 
            
        } 
    } 

    public InventoryItem InventoryItem;
    private List<InvTile> _occupiedTiles = new List<InvTile>();
    public List<InvTile> OccupiedTiles { 
        get { 
            return _occupiedTiles; 
        }
        set {
            _occupiedTiles = value;
        } 
    }


    private void Awake() {
        _containerRectTransform = GetComponent<RectTransform>();
        _image = GetComponentInChildren<Image>();
    }


    public void SetupItem(InventoryItem item, float tileSize, List<InvTile> occupiedTiles) {
        InventoryItem = item;
        _occupiedTiles = occupiedTiles;

        
        _containerRectTransform.sizeDelta = new Vector2(tileSize, tileSize);
        _pivotRectTransform.sizeDelta = new Vector2(tileSize, tileSize);

        // _rectTransform.anchoredPosition = new Vector2(tileSize * InventoryItem.Position.x, -tileSize * InventoryItem.Position.y);
        _containerRectTransform.anchoredPosition = new Vector2(tileSize * InventoryItem.Position.x, -tileSize * InventoryItem.Position.y);

        _pivotRectTransform.rotation = InventoryItem.Rotated ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;

        _imageRectTransform.sizeDelta = new Vector2(tileSize * InventoryItem.Size.x, tileSize * InventoryItem.Size.y);
        _image.sprite = InventoryItem.ItemData.Icon;

        _quantityGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize / 2);
        _amountText = _quantityGameObject.GetComponent<TextMeshProUGUI>();

        if (InventoryItem.Rotated) {
            _imageRectTransform.anchoredPosition = new Vector2(-tileSize * InventoryItem.Size.x + tileSize, 0);
            _quantityGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((InventoryItem.Size.y - 1) * tileSize, (InventoryItem.Size.x - 1) * -tileSize);
        } else {
            _quantityGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((InventoryItem.Size.x - 1) * tileSize, (InventoryItem.Size.y - 1) * -tileSize);
        }

        Amount = InventoryItem.Amount;
    }


    public void OnPointerEnter() {
        _imageRectTransform.localScale = new Vector3(1.1f, 1.1f, 1);
        EventBus.ItemUIHoverEvent.Invoke(InventoryItem);
    }

    public void OnPointerExit() {
        _imageRectTransform.localScale = new Vector3(1, 1, 1);
        EventBus.ItemUIHoverEvent.Invoke(null);
    }

    public void OnPointerDown() {
        _image.color = new Color(1, 1, 1, 0.5f);
    }
    public void OnPointerUp() {
        _image.color = new Color(1, 1, 1, 1);
    }
    public void OnPointerClick() {
        EventBus.ItemUIClickEvent.Invoke(this);
    }

}
