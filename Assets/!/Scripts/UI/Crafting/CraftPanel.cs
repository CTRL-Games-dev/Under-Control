using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftPanel : MonoBehaviour
{
    public List<ConsumableItemData> CraftableItems = new List<ConsumableItemData>();
    public GameObject CraftableItemPrefab;
    public GameObject CraftableItemsHolder;
    // Update is called once per frame
    void Start() {
        foreach (ConsumableItemData item in CraftableItems) {
            GameObject craftableItem = Instantiate(CraftableItemPrefab, CraftableItemsHolder.transform);
            CraftUI craftUI = craftableItem.GetComponent<CraftUI>();
            if (craftUI != null) {
                craftUI.Setup(item);
            } else {
                Debug.LogError("CraftUI component not found on the prefab.");
            }
        }   
    }

    public void UpdateChildren() {
        foreach (Transform child in CraftableItemsHolder.transform) {
            child.GetComponent<CraftUI>().UpdateCanCraftIndicator();
        }
    }
}
