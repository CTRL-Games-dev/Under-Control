using UnityEngine;
using TMPro;

public class ItemEntity : MonoBehaviour
{
    public int Amount;
    public ItemData ItemData;

    [SerializeField] private TMP_Text _title;

    void Start()
    {
        _title.text = ItemData.DisplayName;
    }

    void OnTriggerEnter(Collider other)
    {
        var livingEntity = other.GetComponent<LivingEntity>();
        if(livingEntity == null) {
            return;
        }

        if(!livingEntity.Inventory.AddItem(ItemData, Amount)) {
            return;
        }

        EventBus.OnInventoryItemChanged?.Invoke();
        Destroy(gameObject);
    }
}
