using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItemUI : MonoBehaviour
{
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private RectTransform _amountRectTransform;

    public GameObject Holder;

    public Vector2Int SelectedOffsetInv;

    private InventoryItem _inventoryItem = null;
    public InventoryItem InventoryItem { 
        get { return _inventoryItem; } 
        set {
            if (_inventoryItem != null && value != null) {
                Debug.Log("SelectedItemUI: InventoryItem is already set");
                return;
            }

            SelectedOffsetInv = Vector2Int.zero;

            _inventoryItem = value;
            gameObject.SetActive(_inventoryItem != null);
            if (_inventoryItem == null) {
                EventBus.SelectedItemSet?.Invoke(null);
                

                _image.sprite = null;
                transform.rotation = Quaternion.identity;
                gameObject.SetActive(false);
            } else {
                EventBus.SelectedItemSet?.Invoke(_inventoryItem.ItemData);

                Vector3 itemUiOffset = Vector3.zero;
                if(_inventoryItem.ItemUI != null) {
                    itemUiOffset = _inventoryItem.ItemUI.transform.position - Input.mousePosition;
                    if (_inventoryItem.Rotated) {
                        itemUiOffset = new Vector3(
                            itemUiOffset.y - InventoryPanel.TileSize * value.Size.x * UICanvas.ScreenScale.x,
                            -itemUiOffset.x,
                            0
                        );
                    }
                }

                itemUiOffset = UICanvas.ScaleToCanvas(itemUiOffset);

                Holder.transform.localPosition = itemUiOffset;

                SelectedOffsetInv.x = (int)(itemUiOffset.x / InventoryPanel.TileSize);
                SelectedOffsetInv.y = -(int)(itemUiOffset.y / InventoryPanel.TileSize); // negative = up
                
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
        Player.Instance.ItemRotateEvent.AddListener(OnRotate);
        gameObject.SetActive(false);
    }

    private void LateUpdate()  {
        if(_inventoryItem == null) {
            return;
        }
        
        transform.position = Input.mousePosition;

        if (transform.rotation != _goalRotation) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _goalRotation, _rotationSpeed * Settings.AnimationSpeed * Time.deltaTime);
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
