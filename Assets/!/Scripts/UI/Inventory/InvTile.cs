using System.Collections;
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
            InventoryUIManager.ClearHighlights();
            InventoryUIManager.HighlightNeighbours(Pos, InventoryUIManager.SelectedInventoryItem);
        } else {
            InventoryUIManager.ClearHighlights();
        }
        _highlightImage.SetActive(true);
        
    }
    public void OnPointerExit() {
        if (InventoryUIManager.SelectedTile == this) {
            InventoryUIManager.SelectedTile = null;
            _highlightImage.SetActive(false);
        }
        // StartCoroutine(DeHighlight());
    }
    private IEnumerator DeHighlight() {
        yield return new WaitForSeconds(0.1f);
        if (InventoryUIManager.SelectedTile == this) {
            InventoryUIManager.SelectedTile = null;
            _highlightImage.SetActive(false);
        }
    }

    public void OnPointerClick() {
        InventoryUIManager.TryMoveSelectedItem();
    }
    public void OnDrop() {
        InventoryUIManager.TryMoveSelectedItem();
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
