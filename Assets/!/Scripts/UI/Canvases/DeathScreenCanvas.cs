using UnityEngine;
using DG.Tweening;
using System.Collections;
using Unity.Cinemachine;

public class DeathScreenCanvas : MonoBehaviour, IUICanvasState
{
    [SerializeField] private CinemachineCamera _deathCamera;
    [SerializeField] private Material _dissolveMaterial;
    [SerializeField] private RectTransform _leftHandRect, _rightHandRect;
    [SerializeField] private CanvasGroup _textCanvasGroup;

    private CanvasGroup _canvasGroup;

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
    }


    public void ShowUI() {
        gameObject.SetActive(true);
        
        CameraManager.ShakeCamera(1, 0.1f);
        CameraManager.SwitchCamera(_deathCamera);
        Player.Instance.LockRotation = true;
        Player.Instance.InputDisabled = true;
        Player.Instance.MainCamera.cullingMask = LayerMask.GetMask("Player");

        float dissolve = 1f;
        _dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
        DOTween.To(() => dissolve, x => dissolve = x, 0.365f, 2f).OnUpdate(() => {
            _dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
        }).OnComplete(() => {
            
            DOTween.To(() => dissolve, x => dissolve = x, 0.25f, 3f).OnUpdate(() => {
                _dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
                _canvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed);
            
            }).OnComplete(() => {
                Player.UICanvas.ChangeUIBottomState(UIBottomState.NotVisible);

                DOTween.To(() => dissolve, x => dissolve = x, 0f, 1f).OnUpdate(() => {
                    _dissolveMaterial.SetFloat("_DissolveStrength", dissolve);
                });

                _leftHandRect.DOAnchorPosX(0, 1f * Settings.AnimationSpeed).SetEase(Ease.OutSine);
                _rightHandRect.DOAnchorPosX(-750, 1f * Settings.AnimationSpeed).SetEase(Ease.OutSine).OnComplete(() => {
                    _leftHandRect.DOShakeRotation(0.5f * Settings.AnimationSpeed, 10, 10, 90, false, ShakeRandomnessMode.Harmonic);
                    _rightHandRect.DOShakeRotation(0.5f * Settings.AnimationSpeed, 10, 10, 90, false, ShakeRandomnessMode.Harmonic);

                    _textCanvasGroup.DOFade(1, 0.5f * Settings.AnimationSpeed).OnComplete(() => {
                        transform.DOScale(Vector3.one, 2f).OnComplete(() =>{
                            LoadingScreen.LoadScene("NewHub");
                        });
                    });

                });
            });
        });
    }

    public void HideUI() {
        Player.Instance.MainCamera.cullingMask = ~0; // Reset culling mask to default
        _leftHandRect.DOAnchorPosX(-750, 1f * Settings.AnimationSpeed).SetEase(Ease.OutSine);
        _rightHandRect.DOAnchorPosX(0, 1f * Settings.AnimationSpeed).SetEase(Ease.OutSine);
        _textCanvasGroup.DOFade(0, 0.5f * Settings.AnimationSpeed).OnComplete(() => {
            _canvasGroup.DOFade(0, 0.5f * Settings.AnimationSpeed).OnComplete(() => {
                gameObject.SetActive(false);
            });
        });
    }

    private void OnDisable(){
        _dissolveMaterial.SetFloat("_DissolveStrength", 1);
    }

}
