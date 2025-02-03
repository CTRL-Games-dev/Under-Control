using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InvTile : MonoBehaviour
{
    private GameObject _highlightImage;

    private Image _image;
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


    void Awake() {
        _image = GetComponent<Image>();
        _highlightImage = transform.GetChild(0).gameObject;
    }

    public void OnPointerEnter() {
        InventoryPanel.OnInvTileEnter(this);
        _highlightImage.SetActive(true);
    }
    public void OnPointerExit() {
        InventoryPanel.OnInvTileExit(this);
    }


    public void OnPointerClick() {
        InventoryPanel.TryMoveSelectedItem();
    }
    public void OnDrop() {
        InventoryPanel.TryMoveSelectedItem();
    }

    public void SetHighlight(bool value) {
        _highlightImage.SetActive(value);
    }
}
