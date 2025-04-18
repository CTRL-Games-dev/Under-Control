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

    [SerializeField] private RectTransform _consumable1Rect;
    [SerializeField] private RectTransform _consumable2Rect;

    [SerializeField] private Image _consumable1CooldownImg;
    [SerializeField] private Image _consumable2CooldownImg;

    [Header("Colors")]
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _manaColor;
    [SerializeField] private Color _healthColor;


    private void Start() {
        Player.Instance.UpdateConsumablesEvent.AddListener(OnUpdateConsumables);

        UpdateHealthBar();
        UpdateManaBar();
    }


    public void UpdateHealthBar() {
        _healthBarImg.fillAmount = Player.LivingEntity.Health / Player.LivingEntity.MaxHealth;
        _healthText.text = $"{(int)Player.LivingEntity.Health}/{(int)Player.LivingEntity.MaxHealth}";
    }

    public void UpdateManaBar() {
        _manaBarImg.fillAmount = Player.Instance.Mana / Player.Instance.MaxMana;
        _manaText.text = $"{(int)Player.Instance.Mana}/{(int)Player.Instance.MaxMana}";
    }


    public void OnUpdateConsumables() {
        if (Player.Instance.ConsumableItemOne != null) {
            if (Player.Instance.ConsumableItemOne.ItemData == null) {
                _consumable1Img.gameObject.SetActive(false);
                _consumable1CooldownImg.gameObject.SetActive(false);
                return;
            }
            _consumable1Img.gameObject.SetActive(true);
            _consumable1CooldownImg.gameObject.SetActive(true);
            _consumable1Img.sprite = Player.Instance.ConsumableItemOne.ItemData.Icon;
            _consumable1Text.text = Player.Instance.ConsumableItemOne.Amount.ToString();

            if (Player.Instance.ConsumableItemOne.ItemData is HealthPotionItemData) {
                _consumable1CooldownImg.color = _healthColor;
            } else if (Player.Instance.ConsumableItemOne.ItemData is ManaPotionItemData) {
                _consumable1CooldownImg.color = _manaColor;
            } else {
                _consumable1CooldownImg.color = _defaultColor;
            }

        } else {
            _consumable1Img.sprite = null;
            _consumable1Text.text = "";
            _consumable1Img.gameObject.SetActive(false);
            _consumable1CooldownImg.gameObject.SetActive(false);
        }

        if (Player.Instance.ConsumableItemTwo != null) {
            if (Player.Instance.ConsumableItemTwo.ItemData == null) {
                _consumable2Img.gameObject.SetActive(false);
                _consumable2CooldownImg.gameObject.SetActive(false);
                return;
            }
            _consumable2Img.gameObject.SetActive(true);
            _consumable2CooldownImg.gameObject.SetActive(true);
            _consumable2Img.sprite = Player.Instance.ConsumableItemTwo.ItemData.Icon;
            _consumable2Text.text = Player.Instance.ConsumableItemTwo.Amount.ToString();

            if (Player.Instance.ConsumableItemTwo.ItemData is HealthPotionItemData) {
                _consumable2CooldownImg.color = _healthColor;
            } else if (Player.Instance.ConsumableItemTwo.ItemData is ManaPotionItemData) {
                _consumable2CooldownImg.color = _manaColor;
            } else {
                _consumable2CooldownImg.color = _defaultColor;
            }

        } else {
            _consumable2Img.sprite = null;
            _consumable2Text.text = "";
            _consumable2Img.gameObject.SetActive(false);
            _consumable2CooldownImg.gameObject.SetActive(false);
        }
    }

    public void UseConsumable1() {
        useConsumable(1);
    }

    public void UseConsumable2() {
        useConsumable(2);
    }

    private void useConsumable(int type) {
        RectTransform consumableRect = type == 1 ? _consumable1Rect : _consumable2Rect;

        consumableRect.DOComplete();
        consumableRect.DOKill();

        consumableRect.DOShakeRotation(0.1f * Settings.AnimationSpeed, 10, 10, 90, false);
        consumableRect.DOScale(Vector3.one * 0.95f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            consumableRect.DOScale(Vector3.one, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutQuint);
        });
        
        _consumable1CooldownImg.DOFillAmount(1, 0.05f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            _consumable1CooldownImg.DOFillAmount(0, Player.Instance.ConsumableCooldown.CooldownTime - 0.05f * Settings.AnimationSpeed).SetEase(Ease.Linear);
        });
        _consumable2CooldownImg.DOFillAmount(1, 0.05f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            _consumable2CooldownImg.DOFillAmount(0, Player.Instance.ConsumableCooldown.CooldownTime - 0.05f * Settings.AnimationSpeed).SetEase(Ease.Linear).OnComplete(() => {
                consumableRect.DOShakeRotation(0.1f * Settings.AnimationSpeed, 10, 10, 90, false);
                consumableRect.DOScale(Vector3.one * 1.05f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
                    consumableRect.DOScale(Vector3.one, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutQuint);
                    consumableRect.localRotation = Quaternion.identity;
                });
            });
        });
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
                    _dashCooldownRect.localRotation = Quaternion.identity;
                });
            });
        });
    }
}
