using UnityEngine;
using UnityEngine.UI;



public class InvTile : MonoBehaviour {
    protected GameObject _highlightImage;

    protected Image _image;
    public InventoryPanel InventoryPanel;
    
    private bool _isEmpty = true;
    public bool IsEmpty {
        get { return _isEmpty; }
        set {
            _isEmpty = value;
            if (_isEmpty) {
                _image.color = Color.white;
            } else {
                _image.color = Color.gray;
            }
        }
    }

    public Vector2Int Pos { get; set; }


    protected void Awake() {
        _image = GetComponent<Image>();
        _highlightImage = transform.GetChild(0).gameObject;
    }

    public void OnPointerEnter() {
        if (InventoryPanel == null) {
            return;
        }
        InventoryPanel.OnInvTileEnter(this);
        _highlightImage.SetActive(true);
    }
    public void OnPointerExit() {
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
