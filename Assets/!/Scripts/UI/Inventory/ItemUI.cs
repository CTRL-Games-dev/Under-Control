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
        _amountText = GetComponentInChildren<TextMeshProUGUI>();
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }


    public void SetupItem(InventoryItem item, float tileSize, List<InvTile> occupiedTiles) {
        InventoryItem = item;
        _occupiedTiles = occupiedTiles;
        
        _rectTransform.sizeDelta = new Vector2(tileSize * InventoryItem.Size.x, tileSize * InventoryItem.Size.y);
        _rectTransform.anchoredPosition = new Vector2(tileSize * InventoryItem.Position.x, -tileSize * InventoryItem.Position.y);
        _image.sprite = InventoryItem.Item.Icon;
        
        _amountRectTransform.sizeDelta = new Vector2(tileSize / 2, tileSize / 2);

        Amount = InventoryItem.Amount;
    }


    public void OnPointerEnter() {
        InventoryUIManager.DisplayItemInfo(InventoryItem);
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
        if(InventoryUIManager.SetSelectedInventoryItem(InventoryItem)) {
            _image.raycastTarget = false;
        };
    
    }

}
