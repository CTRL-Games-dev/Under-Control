using UnityEngine;

// Jeszcze nie wiem jak to zrobimy
// Placeholder
public enum ItemType {
    Material,
    Helmet,
    Chestplate,
    Leggings,
    Boots,
    LeftHand,
    RightHand,
    Twohanded,
    Amulet,
    Ring,
    Consumable,
    QuestItem
}

public class ItemManager : MonoBehaviour {
    public ItemData TestItem;
    public ItemData TestItem2;

    public static ItemManager Instance { get; private set; }

    void Start() {
        if(Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

}
