using UnityEngine;
using TMPro;

public class ItemEntity : MonoBehaviour, IInteractable
{
    public int Amount;
    public ItemData ItemData;

    [SerializeField] private TMP_Text _title;

    void Start()
    {
        _title.text = ItemData.DisplayName;
    }

    public void Interact(PlayerController player) {
        if(!player.LivingEntity.Inventory.AddItem(ItemData, Amount)) {
            return;
        }

        EventBus.InventoryItemChangedEvent?.Invoke();
        Destroy(gameObject);
    }
}
