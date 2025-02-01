using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItemUI : MonoBehaviour
{
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private TextMeshProUGUI _amountText;

    private InventoryItem _inventoryItem = null;
    public InventoryItem InventoryItem { 
        get { return _inventoryItem; } 
        set {
            _inventoryItem = value;
            gameObject.SetActive(_inventoryItem != null);
            if (_inventoryItem == null) {

                _image.sprite = null;
                transform.rotation = Quaternion.identity;
                gameObject.SetActive(false);

            } else {
                _uiCanvasParent.ItemInfoPanel.ShowItemInfo(null);

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
            }
        }
    }

    private RectTransform _rectTransform;
    private Image _image;
    private UICanvas _uiCanvasParent;

    private Quaternion _goalRotation;
    [SerializeField] private float _rotationSpeed = 10f;


    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponentInChildren<Image>();
        _uiCanvasParent = FindAnyObjectByType<UICanvas>();
    }

    private void Start() {
        _uiCanvasParent.PlayerController.OnItemRotateEvent.AddListener(Rotate);
    }

    private void Update()  {
        if(_inventoryItem == null) {
            return;
        }
        transform.position = Input.mousePosition;
        if (transform.rotation != _goalRotation) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _goalRotation, _rotationSpeed * Time.deltaTime);
        }
        if (Input.GetMouseButtonUp(0) && _uiCanvasParent.ActiveInventoryPanel != null) {
            _uiCanvasParent.ActiveInventoryPanel.TryMoveSelectedItem();
        }
    }

    public void Rotate() {
        if(_inventoryItem == null) {
            return;
        }
        InventoryItem.Rotated = !InventoryItem.Rotated;
        _goalRotation = InventoryItem.Rotated ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;

        if (_uiCanvasParent.ActiveInventoryPanel != null)  
            _uiCanvasParent.ActiveInventoryPanel.OnInvTileEnter(_uiCanvasParent.ActiveInventoryPanel.SelectedTile);
    }
}
