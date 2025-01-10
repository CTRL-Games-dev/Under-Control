using UnityEngine;
using UnityEngine.UI;

public class InvTile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Image _image;
    private Color _defaultColor;
    public GridManager GridManager { get; set; }
    
    private bool _isEmpty = true;
    public bool IsEmpty {
        get { return _isEmpty; }
        set {
            _isEmpty = value;
            if (_isEmpty) {
                _image.color = Color.white;
            } else {
                _image.color = Color.red;
            }
        }
    }

    public Vector2Int Pos { get; set; }


    void Awake() {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
    }

    public void OnPointerEnter() {
        _image.color = Color.gray;
        GridManager.SelectedTile = this;
    }
    public void OnPointerExit() {
        _image.color = _defaultColor;
        GridManager.SelectedTile = null;
    }
}
