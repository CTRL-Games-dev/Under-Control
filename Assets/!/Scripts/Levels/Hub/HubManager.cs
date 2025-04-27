using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class HubManager : MonoBehaviour
{
    [SerializeField] private Vector3 _spawnPosition = new();
    public static CinemachineCamera MainMenuCamera;
    [SerializeField] private Material _dissolveMaterial;
    [SerializeField] private Transform _eyeTransform, _cylinderTransform;

    private void Awake() {
        MainMenuCamera = GetComponentInChildren<CinemachineCamera>();
    }

    private void Start() {
        CameraManager.SwitchCamera(MainMenuCamera);
        Player.UICanvas.ChangeUIBottomState(UIBottomState.NotVisible);
        Player.UICanvas.ChangeUIMiddleState(UIMiddleState.MainMenu);
        Player.UICanvas.ChangeUITopState(UITopState.NotVisible);
        Player.Instance.MaxCameraDistance = 7f;
        Player.Instance.UpdateDisabled = true;
        Player.Instance.gameObject.transform.position = _spawnPosition;
        Player.LivingEntity.Health = Player.LivingEntity.StartingHealth;
        Player.LivingEntity.Mana = Player.LivingEntity.StartingMana;
        PlayRespawnAnimation();

        GameManager.Instance.ResetInfluence();
        // Player.Animator.SetTrigger("live");
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.F4)) {
            PlayRespawnAnimation();
        }
    }

    public void PlayRespawnAnimation() {

        float dissolve = 1f;
        DOTween.To(() => dissolve, x => dissolve = x, 0f, 2f).SetDelay(1f).OnUpdate(() => {
            _dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
        }).OnComplete(() => {
            Player.Animator.animatePhysics = false;
            Player.Instance.UpdateDisabled = true;
            Player.Instance.gameObject.transform.DOMoveY(-2, 0);
            Player.Animator.SetTrigger("rise");
            Player.Instance.gameObject.transform.DOComplete();

            Player.Instance.gameObject.transform.DOKill();

            dissolve = 0f;
            DOTween.To(() => dissolve, x => dissolve = x, 1f, 4f).SetDelay(1f).OnUpdate(() => {
                _dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
            });
            Player.Instance.gameObject.transform.DOMoveY(1, 2f).SetEase(Ease.OutQuint).OnComplete(() => {
                Player.Animator.SetTrigger("live");
                Player.Instance.UpdateDisabled = false;
                Player.Animator.animatePhysics = true;
                
            });
        });
   
    }
}