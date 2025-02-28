using UnityEngine;

public class ItemEntityManager : MonoBehaviour
{
    [SerializeField] private GameObject _itemEntityPrefab;
    public static ItemEntityManager Instance { get; private set; }

    void Start() {
        if(Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    public ItemEntityManager() {}

    public void SpawnItemEntity(ItemData itemData, int amount, Vector3 position) {
        Debug.Log("Spawning item entity");
        var gameObject = Instantiate(_itemEntityPrefab, position, Quaternion.identity);

        var itemEntity = gameObject.GetComponent<ItemEntity>();
        itemEntity.Amount = amount;
        itemEntity.ItemData = itemData;
    }
}