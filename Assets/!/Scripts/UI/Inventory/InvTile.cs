using UnityEngine;
using UnityEngine.UI;



public class InvTile : MonoBehaviour {
    protected GameObject _highlightImage;
    
    [SerializeField] protected Image _image;
    public InventoryPanel InventoryPanel;
    
    private bool _isEmpty = true;
    public bool IsEmpty {
        get { return _isEmpty; }
        set {
            _isEmpty = value;
            if (_isEmpty && _image != null) {
                _image.color = Color.white;
            } else {
                _image.color = Color.yellow;
            }
        }
    }

    public Vector2Int Pos { get; set; }


    protected void Awake() {
        _highlightImage = transform.GetChild(0).gameObject;
    }

    public void OnPointerEnter() {
        _highlightImage.SetActive(true);
        if (InventoryPanel == null) {
            return;
        }
        InventoryPanel.OnInvTileEnter(this);
        _highlightImage.SetActive(true);
    }
    public void OnPointerExit() {
        _highlightImage.SetActive(false);
        if (InventoryPanel == null) {
            return;
        }
        InventoryPanel.OnInvTileExit(this);
    }


    public void OnPointerClick() {
        if (InventoryPanel == null) {
            return;
        }
        InventoryPanel.TryMoveSelectedItem();
    }
    public void OnDrop() {
        InventoryPanel.TryMoveSelectedItem();
    }

    public void SetHighlight(bool value) {
        _highlightImage.SetActive(value);
    }
}
