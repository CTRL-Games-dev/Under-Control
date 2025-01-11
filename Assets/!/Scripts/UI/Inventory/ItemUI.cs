using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventorySystem;

public class ItemUI : MonoBehaviour
{
    private Image _image;
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
        _rectTransform.anchoredPosition = new Vector2(tileSize * _inventoryItem.position.x, -tileSize * _inventoryItem.position.y);
        _image.sprite = _inventoryItem.item.Icon;
        Amount = _inventoryItem.amount;
    }


    public void OnPointerEnter() {
        GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnPointerExit() {
        GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnPointerDown() {
        _image.color = new Color(1, 1, 1, 0.5f);
    }
    public void OnPointerUp() {
        _image.color = new Color(1, 1, 1, 1);
    }
    public void OnPointerClick() {
        if (FindAnyObjectByType<SelectedItemUI>() == null) {
            Debug.LogError("SelectedItemUI not found");
            return;
        }
        
        FindAnyObjectByType<SelectedItemUI>().InventoryItem = _inventoryItem;
    }
    public void OnDrag() {
        // _rectTransform.anchoredPosition += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 10;
    }
    public void OnEndDrag() {
        // _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x - _rectTransform.anchoredPosition.x % _rectTransform.sizeDelta.x, _rectTransform.anchoredPosition.y - _rectTransform.anchoredPosition.y % _rectTransform.sizeDelta.y);
    }

}
