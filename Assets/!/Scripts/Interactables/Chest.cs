using UnityEngine;

[RequireComponent(typeof(SimpleInventory))]
public class Chest : MonoBehaviour, IInteractableInventory
{
    [SerializeField] private GameObject _uiPrefab;
    private SimpleInventory _inventory;
    private Animator _animator;

    void Awake() {
        _inventory = GetComponent<SimpleInventory>();
        _animator = GetComponent<Animator>();
    }

    public void Interact() {
        _animator.SetTrigger("open");
        Player.UICanvas.InventoryCanvas.SetOtherInventory(_inventory, _uiPrefab, this, "interactable_name_chest_key");
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.Inventory);
    }

    public void EndInteract() {
        _animator.SetTrigger("close");
    }

    public bool FindFirstEmptySpot(ItemData item, int quantity)
    {
        for (int x = 0; x < _inventory.Size.x; x++)
        {
            for(int y = 0; y < _inventory.Size.y; y++)
            {
                Vector2Int position = new(x,y);
                if(_inventory.AddItem(item, quantity, position)) return true;
            }
        }

        return false;
    }
}
