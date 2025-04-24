using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(SimpleInventory))]
public class Seller : MonoBehaviour, IInteractableInventory
{
    [SerializeField] private GameObject _uiPrefab; 
    public SimpleInventory BuyInventory, SellInventory;
    [SerializeField] private FaceAnimator _faceAnimator;
    public CinemachineCamera BuyCamera, SellCamera, CraftCamera;
    [SerializeField] private Transform _pouchTransform;
    [SerializeField] private Animator _pouchAnimator;
    private Animator _animator;
    private CinemachineCamera _previousCamera;

    private readonly int _isTalkingHash = Animator.StringToHash("isTalking");
    private readonly int _sellHash = Animator.StringToHash("sell");

    private bool _isTallking = false;


    void Awake() {
        _animator = GetComponent<Animator>();
    }


    public Tween EnlargePouch() {
        _pouchAnimator.DOKill();
        _pouchAnimator.SetBool("ensmall", false);
        _pouchAnimator.transform.DORotate(new Vector3(60f, -135f, 0), 2f).OnComplete(() => {
            _pouchAnimator.SetTrigger("enlarge");
        });
        return _faceAnimator.transform.DOScale(Vector3.one, 2f);
    }

    public Tween EnsmallPouch() {
        _pouchAnimator.DOKill();
        _pouchAnimator.SetBool("enlarge", false);
        _pouchAnimator.SetTrigger("ensmall");
        return _pouchAnimator.transform.DORotate(new Vector3(0, -135f, 0), 0.5f).SetDelay(0.5f);
    }


    public void Interact() {
        
        // Debug.Log("Interact with seller");
        _previousCamera = CameraManager.GetCurrentCamera();
        Player.UICanvas.InventoryCanvas.SetSellerTab(this);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.Inventory);
        _isTallking = true;
        _animator.SetBool(_isTalkingHash, _isTallking);
        _animator.SetTrigger(_sellHash);
        _faceAnimator.StartInfiniteAnimation("TALK"); 
        EventBus.InventoryClosedEvent.AddListener(EndInteract);
    }

    public void EndInteract() {
        Player.Instance.UICancelEvent.RemoveListener(EndInteract);
        Player.UICanvas.InventoryCanvas.SetSellerTab(null);
        _faceAnimator.EndAnimation();
        CameraManager.SwitchCamera(_previousCamera);
    }
}
