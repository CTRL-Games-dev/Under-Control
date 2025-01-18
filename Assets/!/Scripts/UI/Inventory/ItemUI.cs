using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventorySystem;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private RectTransform _amountRectTransform;
    public InventoryUIManager InventoryUIManager;

    private Image _image;
    public Image Image { get => _image; }
    private RectTransform _rectTransform;
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

    private InventoryItem _inventoryItem;
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
        _amountText = GetComponentInChildren<TextMeshProUGUI>();
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }


    public void SetupItem(InventoryItem item, float tileSize, List<InvTile> occupiedTiles) {
        _inventoryItem = item;
        _occupiedTiles = occupiedTiles;
        
        _rectTransform.sizeDelta = new Vector2(tileSize * _inventoryItem.Size.x, tileSize * _inventoryItem.Size.y);
        _rectTransform.anchoredPosition = new Vector2(tileSize * _inventoryItem.Position.x, -tileSize * _inventoryItem.Position.y);
        _image.sprite = _inventoryItem.Item.Icon;
        
        _amountRectTransform.sizeDelta = new Vector2(tileSize / 2, tileSize / 2);

        Amount = _inventoryItem.Amount;
    }


    public void OnPointerEnter() {
        InventoryUIManager.DisplayItemInfo(_inventoryItem);
    }

    public void OnPointerExit() {
        InventoryUIManager.DisplayItemInfo(null);
    }

    public void OnPointerDown() {
        _image.color = new Color(1, 1, 1, 0.5f);
    }
    public void OnPointerUp() {
        _image.color = new Color(1, 1, 1, 1);
    }
    public void OnPointerClick() {
        if(InventoryUIManager.SetSelectedInventoryItem(_inventoryItem)) {
            _image.raycastTarget = false;
        };
    
    }

}
