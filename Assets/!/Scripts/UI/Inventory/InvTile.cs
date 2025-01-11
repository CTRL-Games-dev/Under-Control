using UnityEngine;
using UnityEngine.UI;

public class InvTile : MonoBehaviour
{
    private GameObject _highlightImage;

    private Image _image;
    public InventoryUIManager InventoryUIManager;
    
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
        InventoryUIManager.SelectedTile = this;
        if (InventoryUIManager.SelectedInventoryItem != null) {
            InventoryUIManager.HighlightNeighbours(Pos, InventoryUIManager.SelectedInventoryItem.Size);
        }
        
        _highlightImage.SetActive(true);
    }
    public void OnPointerExit() {
        InventoryUIManager.SelectedTile = null;
        InventoryUIManager.ClearHighlights();
    }
    public void OnPointerClick() {
        InventoryUIManager.TryMoveSelectedItem();
    }
    public void OnDrop() {
        Debug.Log("Dropped on " + Pos);
    }

    public void SetHighlight(bool value) {
        // if (!IsEmpty) {
        //     _highlightImage.GetComponent<Image>().color = Color.red;
        // } else {
        //     _highlightImage.GetComponent<Image>().color = Color.green;
        // }
        _highlightImage.SetActive(value);
    }
}
