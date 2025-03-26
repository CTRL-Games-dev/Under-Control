using UnityEngine;

public class HUDCanvas : MonoBehaviour
{
    public void ShowUI() {
        gameObject.SetActive(true);
    }

    public void HideUI() {
        gameObject.SetActive(false);
    }
}
