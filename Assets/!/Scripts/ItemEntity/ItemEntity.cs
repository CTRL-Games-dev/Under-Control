using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

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

        if(itemData.Model != null) {
            Instantiate(itemData.Model, itemEntity.transform);
        } else {
            Instantiate(GameManager.Instance.UnknownModelPrefab, itemEntity.transform);
        }

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
    public Rigidbody Rigidbody { get; private set; }

    public static ItemEntity Spawn(ItemData itemData, int amount, Vector3 position, Quaternion quaternion) {
        ItemEntity itemEntity = Instantiate(GameManager.Instance.ItemEntityPrefab, position, quaternion);
        itemEntity.Amount = amount;
        itemEntity.ItemData = itemData;

        if(itemData.Model != null) {
            Instantiate(itemData.Model, itemEntity.transform);
        } else {
            Instantiate(GameManager.Instance.UnknownModelPrefab, itemEntity.transform);
        }

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
