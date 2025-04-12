using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTooltip : MonoBehaviour {
    [Header("Tooltips")]
    public ItemEntityTooltip ItemEntityTooltip;
    public LivingEntityTooltip LivingEntityTooltip;
    public GenericInteractableTooltip GenericInteractableTooltip;

    private GameObject _lastHoveredGameObject = null;
    private Camera _camera;

    void Start() {
        _camera = Player.Instance.MainCamera;
    }

    void FixedUpdate() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            if (_lastHoveredGameObject != null) {
                _lastHoveredGameObject = null;
                ItemEntityTooltip.Disable();
                LivingEntityTooltip.Disable();
                GenericInteractableTooltip.Disable();
            }
            return;
        }

        transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100)) {
            return;
        }

        if (hit.transform.gameObject == _lastHoveredGameObject) {
            return;
        }

        _lastHoveredGameObject = hit.transform.gameObject;

        ItemEntityTooltip.Disable();
        LivingEntityTooltip.Disable();
        GenericInteractableTooltip.Disable();

        if(hit.transform.gameObject.TryGetComponent(out ItemEntity itemEntity)) {
            ItemEntityTooltip.Enable(itemEntity);
        } else if(hit.transform.gameObject.TryGetComponent(out LivingEntity livingEntity)) {
            if(livingEntity == Player.LivingEntity) {
                return;
            }

            LivingEntityTooltip.Enable(livingEntity);
        } else if(hit.transform.gameObject.TryGetComponent(out IInteractable _)) {
            GenericInteractableTooltip.Enable(null);
        }
    }
}