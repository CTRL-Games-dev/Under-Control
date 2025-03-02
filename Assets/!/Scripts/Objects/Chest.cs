using UnityEngine;

[RequireComponent(typeof(SimpleInventory))]
public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _uiPrefab;
    private SimpleInventory _inventory;

    void Awake() {
        _inventory = GetComponent<SimpleInventory>();
    }

    public void Interact(PlayerController player)
    {
        UICanvas.Instance.SetOtherInventory(_inventory.ItemContainer, _uiPrefab);
    }
}
