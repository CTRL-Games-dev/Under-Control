using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

[RequireComponent(typeof(Rigidbody))]
public class ItemEntity : MonoBehaviour, IInteractable
{
    public int Amount;
    public ItemData ItemData;
    public float PowerScale;

    public Rigidbody Rigidbody { get; private set; }

    public static ItemEntity[] Spawn(ItemData itemData, int amount, Vector3 position, float powerScale, Quaternion quaternion) {
        if(amount <= 0) return new ItemEntity[0];

        int itemEntitiesCount = amount / itemData.MaxQuantity;
        if(amount % itemData.MaxQuantity != 0) {
            itemEntitiesCount++;
        }

        ItemEntity[] itemEntities = new ItemEntity[itemEntitiesCount];

        for(int i = 0; i < itemEntitiesCount; i++) {
            int itemEntityAmount = itemData.MaxQuantity;
            if(i == itemEntitiesCount - 1 && amount % itemData.MaxQuantity != 0) {
                itemEntityAmount = amount % itemData.MaxQuantity;
            }

            ItemEntity itemEntity = spawn(itemData, itemEntityAmount, position, powerScale, quaternion);

            itemEntities[i] = itemEntity;
        }

        return itemEntities;
    }

    public static ItemEntity[] Spawn(ItemData itemData, int amount, Vector3 position, float powerScale) {
        return Spawn(itemData, amount, position, powerScale, Quaternion.identity);
    }

    private static ItemEntity spawn(ItemData itemData, int amount, Vector3 position, float powerScale, Quaternion quaternion) {
        ItemEntity itemEntity = Instantiate(GameManager.Instance.ItemEntityPrefab, position, quaternion);
        itemEntity.Amount = amount;
        itemEntity.ItemData = itemData;
        itemEntity.PowerScale = powerScale;

        if(itemData.Model != null) {
            Instantiate(itemData.Model, itemEntity.transform);
        } else {
            Instantiate(GameManager.Instance.UnknownModelPrefab, itemEntity.transform);
        }

        return itemEntity;
    }

    public static ItemEntity[] SpawnThrownRelative(ItemData itemData, int amount, Vector3 position, float powerScale, Quaternion quaternion, Vector3 force) {
        ItemEntity[] itemEntities = Spawn(itemData, amount, position, powerScale, quaternion);
        
        for(int i = 0; i < itemEntities.Length; i++) {
            itemEntities[i].Rigidbody.AddRelativeForce(force, ForceMode.Impulse);
        }

        return itemEntities;
    }

    void Awake() {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Interact() {
        if(!Player.LivingEntity.Inventory.AddItem(ItemData, Amount, 1)) {
            return;
        }

        Player.UICanvas.PickupItemNotify(ItemData, Amount);
        EventBus.InventoryItemChangedEvent?.Invoke();
        Destroy(gameObject);
    }
}
