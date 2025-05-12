using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTooltip : MonoBehaviour {
    [Header("Tooltips")]
    public ItemEntityTooltip ItemEntityTooltip;
    public LivingEntityTooltip LivingEntityTooltip;
    public GenericInteractableTooltip GenericInteractableTooltip;
    public TalkableTooltip TalkableTooltip;

    private RectTransform _rectTransform;
    private LayerMask _playerMask;
    private LayerMask _interactableMask;

    private GameObject _lastHoveredGameObject = null;
    private Camera _camera;

    void Start() {
        _rectTransform = GetComponent<RectTransform>();

        _camera = Player.Instance.MainCamera;
        _playerMask |= 1 << LayerMask.NameToLayer("Player");
        _playerMask |= 1 << LayerMask.NameToLayer("Hitboxes");
        _interactableMask |= 1 << LayerMask.NameToLayer("Interactable");
    }

    void LateUpdate() {
        if(!InputUtility.IsMousePositionAvailable()) return;

        _rectTransform.anchoredPosition = UICanvas.ScaleToCanvas(new Vector2(Input.mousePosition.x, Input.mousePosition.y)) - new Vector2(0, 1080);
    }

    void FixedUpdate() {
        if(!InputUtility.IsMousePositionAvailable()) return;
       
        if (EventSystem.current.IsPointerOverGameObject()) {
            if (_lastHoveredGameObject != null) {
                _lastHoveredGameObject = null;
                ItemEntityTooltip.Disable();
                LivingEntityTooltip.Disable();
                GenericInteractableTooltip.Disable();
                TalkableTooltip.Disable();
            }
            return;
        }

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit interactableHit, 100, _interactableMask)) {
            if (interactableHit.transform.gameObject == _lastHoveredGameObject) {
                return;
            }

            ItemEntityTooltip.Disable();
            LivingEntityTooltip.Disable();
            GenericInteractableTooltip.Disable();
            TalkableTooltip.Disable();

            if(interactableHit.transform.gameObject.TryGetComponent(out ItemEntity itemEntity)) {
                ItemEntityTooltip.Enable(itemEntity);
                _lastHoveredGameObject = interactableHit.transform.gameObject;
            } else if(interactableHit.transform.gameObject.TryGetComponent(out Talkable _)) {
                TalkableTooltip.Enable(null);
                _lastHoveredGameObject = interactableHit.transform.gameObject;
            } else if(interactableHit.transform.gameObject.TryGetComponent(out IInteractable _)) {
                GenericInteractableTooltip.Enable(null);
                _lastHoveredGameObject = interactableHit.transform.gameObject;
            } else {
                Debug.LogError($"Unknown interactable type: {interactableHit.transform.gameObject.name}");
            }

            return;
        }

        if (!Physics.Raycast(ray, out RaycastHit hit, 100, ~_playerMask)) {
            return;
        }

        if (hit.transform.gameObject == _lastHoveredGameObject) {
            return;
        }

        ItemEntityTooltip.Disable();
        LivingEntityTooltip.Disable();
        GenericInteractableTooltip.Disable();
        TalkableTooltip.Disable();

        if(hit.transform.gameObject.TryGetComponentInParent(out LivingEntity livingEntity)) {
            if(livingEntity == Player.LivingEntity) {
                return;
            }

            if(livingEntity.IsInvisible) {
                return;
            }

            LivingEntityTooltip.Enable(livingEntity);
        }

        _lastHoveredGameObject = hit.transform.gameObject;
    }
}