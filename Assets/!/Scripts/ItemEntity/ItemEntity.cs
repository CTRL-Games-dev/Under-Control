using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemEntity : MonoBehaviour, IInteractable
{
    public int Amount;
    public ItemData ItemData;

    public Rigidbody Rigidbody { get; private set; }

    public static ItemEntity Spawn(ItemData itemData, int amount, Vector3 position, Quaternion quaternion) {
        ItemEntity itemEntity = Instantiate(GameManager.Instance.ItemEntityPrefab, position, quaternion);
        itemEntity.Amount = amount;
        itemEntity.ItemData = itemData;

        GameObject model;
        if(itemData.Model != null) {
            model = Instantiate(itemData.Model, itemEntity.transform);
        } else {
            model = Instantiate(GameManager.Instance.UnknownModelPrefab, itemEntity.transform);
        }

        // Apply item entity layer to model
        model.layer = itemEntity.gameObject.layer;

        return itemEntity;
    }

    public static ItemEntity Spawn(ItemData itemData, int amount, Vector3 position) {
        return Spawn(itemData, amount, position, Quaternion.identity);
    }

    public static ItemEntity SpawnThrownRelative(ItemData itemData, int amount, Vector3 position, Quaternion quaternion, Vector3 force) {
        ItemEntity itemEntity = Spawn(itemData, amount, position, quaternion);
        itemEntity.Rigidbody.AddRelativeForce(force, ForceMode.Impulse);
        return itemEntity;
    }

    void Awake() {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Interact(PlayerController player) {
        if(!player.LivingEntity.Inventory.AddItem(ItemData, Amount)) {
            return;
        }

        EventBus.InventoryItemChangedEvent?.Invoke();
        Destroy(gameObject);
    }
}
