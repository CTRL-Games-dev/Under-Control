using UnityEngine;

public class SlashScaler : MonoBehaviour
{
    public GameObject SlashGO;

    // Update is called once per frame
    void Update() {
        if (transform.localScale.x < 0.001f) SlashGO.SetActive(false);
        else SlashGO.SetActive(true);
    }
}
