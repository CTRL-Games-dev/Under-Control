using UnityEngine;
using TMPro;

public class ItemEntity : MonoBehaviour
{
    public int Amount;
    public Item Item;

    [SerializeField] private TMP_Text _title;

    void Start()
    {
        _title.text = Item.DisplayName;
    }

    void OnTriggerEnter(Collider other)
    {
        var livingEntity = other.GetComponent<LivingEntity>();
        if(livingEntity == null) {
            return;
        }

        if(!livingEntity.InventorySystem.AddItem(Item, Amount)) {
            return;
        }

        EventBus.OnInventoryItemChanged?.Invoke();
        Destroy(gameObject);
    }
}
