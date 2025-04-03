using UnityEngine;

public class HUDCanvas : MonoBehaviour, IUICanvasState
{
    public void ShowUI() {
        gameObject.SetActive(true);
    }

    public void HideUI() {
        gameObject.SetActive(false);
    }
}
