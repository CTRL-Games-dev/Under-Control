using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if(Vector3.Distance(player.transform.position, transform.position) > 3f) 
        {
            return;
        }

        if(!player.LivingEntity.Inventory.AddItem(ItemData, Amount)) 
        {
            return;
        }

        EventBus.InventoryItemChangedEvent?.Invoke();
        Destroy(gameObject);
    }

    
}
