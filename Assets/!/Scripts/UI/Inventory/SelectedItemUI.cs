using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItemUI : MonoBehaviour
{
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private RectTransform _amountRectTransform;

    private InventoryItem _inventoryItem = null;
    public InventoryItem InventoryItem { 
        get { return _inventoryItem; } 
        set {
            if (_inventoryItem != null && value != null) {
                Debug.Log("SelectedItemUI: InventoryItem is already set");
                return;
            }

            _inventoryItem = value;
            gameObject.SetActive(_inventoryItem != null);
            if (_inventoryItem == null) {
                _image.sprite = null;
                transform.rotation = Quaternion.identity;
                gameObject.SetActive(false);
            } else {
                Player.UICanvas.ItemInfoPanel.ShowItemInfo(null);

                transform.rotation = _inventoryItem.Rotated ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;
                _goalRotation = transform.rotation; 
                transform.position = Input.mousePosition;

                gameObject.SetActive(true);

                _rectTransform.sizeDelta = new Vector2(InventoryPanel.TileSize, InventoryPanel.TileSize);
                _imageRectTransform.sizeDelta = new Vector2(InventoryPanel.TileSize * _inventoryItem.Size.x, InventoryPanel.TileSize * _inventoryItem.Size.y);

                _image.sprite = _inventoryItem.ItemData.Icon;
                if (_inventoryItem.Amount == 1) {
                    _amountText.text = "";
                    return;
                }
                _amountText.text = _inventoryItem.Amount.ToString();
                _amountRectTransform.anchoredPosition = new Vector2((_inventoryItem.Size.x -1) * InventoryPanel.TileSize, (_inventoryItem.Size.y - 1) * -InventoryPanel.TileSize);
            }
        }
    }

    private RectTransform _rectTransform;
    private Image _image;

    private Quaternion _goalRotation;
    [SerializeField] private float _rotationSpeed = 10f;


    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponentInChildren<Image>();
    }

    private void Start() {
        Player.PlayerController.ItemRotateEvent.AddListener(OnRotate);
    }

    private void Update()  {
        if(_inventoryItem == null) {
            return;
        }
        
        transform.position = Input.mousePosition;

        if (transform.rotation != _goalRotation) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _goalRotation, _rotationSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButtonUp(0) && Player.UICanvas.ActiveInventoryPanel != null) {
            Player.UICanvas.ActiveInventoryPanel.TryMoveSelectedItem();
        }
    }

    public void OnRotate() {
        if(_inventoryItem == null) {
            return;
        }

        InventoryItem.Rotated = !InventoryItem.Rotated;
        _goalRotation = InventoryItem.Rotated ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;

        if (Player.UICanvas.ActiveInventoryPanel != null)  
            Player.UICanvas.ActiveInventoryPanel.OnInvTileEnter(Player.UICanvas.ActiveInventoryPanel.SelectedTile);
    }

    public void UpdateAmount() {
        if (_inventoryItem == null) return;
        if (_inventoryItem.Amount == 0) {
            InventoryItem = null;
            return;
        }
        if (_inventoryItem.Amount == 1) {
            _amountText.text = "";
            return;
        }
        _amountText.text = _inventoryItem.Amount.ToString(); 
    }
}
