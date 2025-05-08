using UnityEngine;

[RequireComponent(typeof(SimpleInventory))]
public class Chest : MonoBehaviour, IInteractableInventory
{
    [SerializeField] private GameObject _uiPrefab;
    public SimpleInventory Inventory;
    private Animator _animator;

    void Awake() {
        Inventory = GetComponent<SimpleInventory>();
        _animator = GetComponent<Animator>();
    }

    public void Interact() {
        _animator.SetTrigger("open");
        Player.UICanvas.InventoryCanvas.SetOtherInventory(Inventory, _uiPrefab, this, "interactable_name_chest_key", false, true);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.Inventory);
    }

    public void EndInteract() {
        _animator.SetTrigger("close");
    }
}
