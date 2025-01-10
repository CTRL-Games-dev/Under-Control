using UnityEngine;

public class ItemUI : MonoBehaviour
{
    public void OnPointerEnter() {
        GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnPointerExit() {
        GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
    }
}
