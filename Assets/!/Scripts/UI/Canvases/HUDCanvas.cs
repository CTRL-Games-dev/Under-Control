using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUDCanvas : MonoBehaviour, IUICanvasState
{
    public void ShowUI() {
        gameObject.SetActive(true);
    }

    public void HideUI() {
        gameObject.SetActive(false);
    }

    [SerializeField] private Image _healthBarImg;
    [SerializeField] private Image _manaBarImg;

    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _manaText;

    [SerializeField] private Image _dashCooldownImg;
    [SerializeField] private RectTransform _dashCooldownRect;


    [Header("Consumables")]

    [SerializeField] private Image _consumable1Img;
    [SerializeField] private Image _consumable2Img;

    [SerializeField] private TextMeshProUGUI _consumable1Text;
    [SerializeField] private TextMeshProUGUI _consumable2Text;


    private void Update() {
        UpdateHealthBar();
        UpdateManaBar();
    }

    private void UpdateHealthBar() {
        _healthBarImg.fillAmount = Player.LivingEntity.Health / Player.LivingEntity.MaxHealth;
        _healthText.text = $"{(int)Player.LivingEntity.Health}/{(int)Player.LivingEntity.MaxHealth}";
    }

    private void UpdateManaBar() {
        _manaBarImg.fillAmount = Player.Instance.Mana / Player.Instance.MaxMana;
        _manaText.text = $"{(int)Player.Instance.Mana}/{(int)Player.Instance.MaxMana}";
    }

    public void ShowDashCooldown() {
        _dashCooldownRect.DOKill();
        _dashCooldownImg.DOKill();
        _dashCooldownRect.DOShakeRotation(0.1f * Settings.AnimationSpeed, 10, 10, 90, false);
        _dashCooldownRect.DOScale(Vector3.one * 0.95f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            _dashCooldownRect.DOScale(Vector3.one, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutQuint);
        });
        _dashCooldownImg.DOFillAmount(1, 0.05f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            _dashCooldownImg.DOFillAmount(0, Player.Instance.DashCooldown - 0.05f * Settings.AnimationSpeed).SetEase(Ease.Linear).OnComplete(() => {
                _dashCooldownRect.DOShakeRotation(0.1f * Settings.AnimationSpeed, 10, 10, 90, false);
                _dashCooldownRect.DOScale(Vector3.one * 1.05f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
                    _dashCooldownRect.DOScale(Vector3.one, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutQuint);
                });
            });
        });
    }
}
