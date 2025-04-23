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

    private readonly int _isTalkingHash = Animator.StringToHash("isTalking");
    private readonly int _sellHash = Animator.StringToHash("sell");

    private bool _isTallking = false;
    private bool _enlarged = false;


    void Awake() {
        _animator = GetComponent<Animator>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F3)) {
            _faceAnimator.StartAnimation("TALK", 5f); 
        }


                if (Input.GetKeyUp(KeyCode.Alpha7)) {
            _isTallking = !_isTallking;
            _animator.SetBool(_isTalkingHash, _isTallking);
        }

    }

    public Tween EnlargePouch() {
        _pouchAnimator.DOKill();
        _pouchAnimator.transform.DORotate(new Vector3(60f, -135f, 0), 2f).OnComplete(() => {
            if (!_enlarged) {
                _pouchAnimator.SetTrigger("enlarge");
                _enlarged = true;
            }
        });
        return _faceAnimator.transform.DOScale(Vector3.one, 2f);
    }

    public Tween ShrinkPouch() {
        _pouchAnimator.DOKill();
        if (_enlarged) {
            _pouchAnimator.SetTrigger("enlarge");
            _enlarged = false;
        }
        return _pouchAnimator.transform.DORotate(new Vector3(0, -135f, 0), 0.5f);
    }


    public void Interact() {
        
        // Debug.Log("Interact with seller");
        Player.UICanvas.InventoryCanvas.SetSellerTab(this);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.Inventory);
        _isTallking = true;
        _animator.SetBool(_isTalkingHash, _isTallking);
        _animator.SetTrigger(_sellHash);

        Player.Instance.UICancelEvent.AddListener(EndInteract);
    }

    public void EndInteract() {
        Player.Instance.UICancelEvent.RemoveListener(EndInteract);
        CameraManager.Instance.SwitchCamera(null);
    }
}
