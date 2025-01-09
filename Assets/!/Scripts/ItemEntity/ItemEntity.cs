using UnityEngine;
using TMPro;

public class ItemEntity : MonoBehaviour
{
    public int Amount;
    public Item Item;

    [SerializeField] private TMP_Text title;

    void Start()
    {
        title.text = Item.DisplayName;
    }

    void OnTriggerEnter(Collider other)
    {
        var livingEntity = other.GetComponent<LivingEntity>();
        if(livingEntity == null) {
            return;
        }

        if(!livingEntity.inventorySystem.AddItem(Item, Amount)) {
            return;
        }

        Destroy(gameObject);
    }
}
