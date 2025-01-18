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

    public void SpawnItemEntity(Item item, int amount, Vector3 position) {
        var gameObject = Instantiate(_itemEntityPrefab, position, Quaternion.identity);

        var itemEntity = gameObject.GetComponent<ItemEntity>();
        itemEntity.Amount = amount;
        itemEntity.Item = item;
    }
}