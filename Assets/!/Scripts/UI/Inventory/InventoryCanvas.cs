using UnityEngine;

public class InventoryCanvas : MonoBehaviour
{
    private UICanvas _uiCanvas;

    private void Awake() {
    }

    void Start() {
        _uiCanvas = UICanvas.Instance;
        
    }


    public void OnBackgroundClick() {
        _uiCanvas.DropItem();
    }
}
