using UnityEngine;

// Jeszcze nie wiem jak to zrobimy
// Placeholder
public class ItemManager : MonoBehaviour
{
    public Item TestItem;
    public Item TestItem2;

    public static ItemManager Instance { get; private set; }

    void Start() {
        if(Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

}
