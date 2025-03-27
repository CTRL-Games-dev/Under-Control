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
        Player.UICanvas.SetOtherInventory(_inventory.ItemContainer, _uiPrefab, this, "CHEST");
    }

    public void EndInteract() {
        _animator.SetTrigger("close");
    }
}
