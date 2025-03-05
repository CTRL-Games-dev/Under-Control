using UnityEngine;

public class ItemEntity : MonoBehaviour, IInteractable
{
    public int Amount;
    public ItemData ItemData;

    public static ItemEntity Spawn(ItemData itemData, int amount, Vector3 position) {
        ItemEntity itemEntity = Instantiate(GameManager.Instance.ItemEntityPrefab, position, Quaternion.identity);
        itemEntity.Amount = amount;
        itemEntity.ItemData = itemData;

        if(itemData.Model != null) {
            Instantiate(itemData.Model, itemEntity.transform);
        } else {
            Instantiate(GameManager.Instance.UnknownModelPrefab, itemEntity.transform);
        }

        return itemEntity;
    }

    public void Interact(PlayerController player) {
        if(!player.LivingEntity.Inventory.AddItem(ItemData, Amount)) {
            return;
        }

        EventBus.InventoryItemChangedEvent?.Invoke();
        Destroy(gameObject);
    }
}
