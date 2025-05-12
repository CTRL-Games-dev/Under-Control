using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;

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

    [Header("Active Effects")]
    [SerializeField] private GameObject _effectsHolder;
    [SerializeField] private GameObject _effectPrefab;
    [SerializeField] private RectTransform _moreInfoRect;
    [SerializeField] private TextMeshProUGUI _moreInfoText;
    [SerializeField] private TextLocalizer _nameTextLocalizer;
    [SerializeField] private TextMeshProUGUI _durationText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private GameObject _infinityGO;

    private List<EffectUI> _handledEffects = new();
    

    [Header("Spell Slots")]
    [SerializeField] private Image _spellSlot1Icon;
    [SerializeField] private Image _spellSlot2Icon;
    [SerializeField] private Image _spellSlot3Icon;
    [SerializeField] private Image _spellSlot1CooldownImg;
    [SerializeField] private Image _spellSlot2CooldownImg;
    [SerializeField] private Image _spellSlot3CooldownImg;
    [SerializeField] private RectTransform _spellSlot1Rect;
    [SerializeField] private RectTransform _spellSlot2Rect;
    [SerializeField] private RectTransform _spellSlot3Rect;

    [Header("Rebinding")]
    [SerializeField] private Button _rebindEventButton;
    [SerializeField] private List<BindReferences> _bindReferences = new();
    [SerializeField] private Image _keyConsumable1Img, _keyConsumable2Img, _keySpell1Img, _keySpell2Img, _keySpell3Img;

    [Header("Boss Bar")]
    [SerializeField] private Image _bossBarImg;
    [SerializeField] private TextLocalizer _bossNameTextLocalizer;
    [SerializeField] private GameObject _bossBarGO;

    private LivingEntity _bossEntity;
    private float _previousBossHealth;
     
    [Serializable] 
    public struct BindReferences {
        public TextMeshProUGUI HUDText, BindingText;
    } 

    private float _previousHealth;
    private float _previousMana;



    private void Start() {
        Player.Instance.UpdateConsumablesEvent.AddListener(OnUpdateConsumables);
        _rebindEventButton.onClick.AddListener(updateBindingTextsLate);
        EventBus.BindingsChangedEvent.AddListener(updateBindingTextsLate);

        _previousHealth = Player.LivingEntity.Health;
        _previousMana = Player.Instance.Mana;

        UpdateHealthBar();
        UpdateManaBar();
        updateBindingTexts();
    }

    private void FixedUpdate() {
        if (Player.Instance == null) return;
        
        List<LivingEntity.EffectData> activeEffects = Player.LivingEntity.ActiveEffects;
        if (activeEffects.Count == 0) return;

        List<EffectUI> handledEffects = _handledEffects;
        foreach (LivingEntity.EffectData effectData in activeEffects) {
            if (handledEffects.Count == 0) {
                handleEffect(effectData);
            } else {
                bool isHandled = false;
                foreach (EffectUI handledEffect in handledEffects) {
                    if (effectData.Equals(handledEffect.EffectData)) {
                        isHandled = true;
                        break;
                    }
                }
                if (!isHandled) handleEffect(effectData);
            }
        }
    }

    private void updateBindingTextsLate() {
        Invoke(nameof(updateBindingTexts), 0.1f);
    }

    private void updateBindingTexts() {
        foreach (BindReferences bindReference in _bindReferences) {
            bindReference.HUDText.text = bindReference.BindingText.text;
        }
    }


    private void handleEffect(LivingEntity.EffectData effectData) {
        EffectUI effectUI = Instantiate(_effectPrefab, _effectsHolder.transform).GetComponent<EffectUI>();
        effectUI.Setup(effectData);
        _handledEffects.Add(effectUI);
    }

    public void RemoveEffectUI(EffectUI effectUI) {
        _handledEffects.Remove(effectUI);
    }


    public void UpdateHealthBar() {
        float health = _previousHealth;
        DOTween.Complete(health);
        DOTween.Kill(health);
        DOTween.To(() => health, x => health = x, Player.LivingEntity.Health, 0.3f * Settings.AnimationSpeed).SetEase(Ease.OutCubic).OnUpdate(() => {
            _healthBarImg.fillAmount = health / Player.LivingEntity.MaxHealth;
            _healthText.text = $"{(int)Math.Ceiling(health)}/{(int)Player.LivingEntity.MaxHealth}";
        });

        _previousHealth = Player.LivingEntity.Health;
    }

    public void UpdateManaBar() {
        float mana = _previousMana;
        DOTween.Complete(mana);
        DOTween.Kill(mana);
        DOTween.To(() => mana, x => mana = x, Player.Instance.Mana, 0.3f * Settings.AnimationSpeed).SetEase(Ease.OutCubic).OnUpdate(() => {
            _manaBarImg.fillAmount = mana / Player.Instance.MaxMana;
            _manaText.text = $"{(int)mana}/{(int)Player.Instance.MaxMana}";
        });

        _previousMana = Player.Instance.Mana;
    }

    public void UpdateSpellSlots() {
        if (Player.Instance.SpellSlotOne != null) {
            _spellSlot1Icon.sprite = Player.Instance.SpellSlotOne.Icon;
            _spellSlot1Icon.gameObject.SetActive(true);
        } else {
            _spellSlot1Icon.gameObject.SetActive(false);
        }

        if (Player.Instance.SpellSlotTwo != null) {
            _spellSlot2Icon.sprite = Player.Instance.SpellSlotTwo.Icon;
            _spellSlot2Icon.gameObject.SetActive(true);
        } else {
            _spellSlot2Icon.gameObject.SetActive(false);
        }

        if (Player.Instance.SpellSlotThree != null) {
            _spellSlot3Icon.sprite = Player.Instance.SpellSlotThree.Icon;
            _spellSlot3Icon.gameObject.SetActive(true);
        } else {
            _spellSlot3Icon.gameObject.SetActive(false);
        }
    }

    public void SetSpellCooldownColor(int spellSlot, Color color) {
        Color c = color;
        c.a = 75f / 255f;
        switch (spellSlot) {
            case 1:
                _spellSlot1CooldownImg.color = c;
                break;
            case 2:
                _spellSlot2CooldownImg.color = c;
                break;
            case 3:
                _spellSlot3CooldownImg.color = c;
                break;
        }
    }

    public void UseSpell1() {
        useSpell(_spellSlot1CooldownImg, _spellSlot1Rect, Player.Instance.SpellSlotOne, _keySpell1Img);
    }

    public void UseSpell2() {
        useSpell(_spellSlot2CooldownImg, _spellSlot2Rect, Player.Instance.SpellSlotTwo, _keySpell2Img);
    }

    public void UseSpell3() {
        useSpell(_spellSlot3CooldownImg, _spellSlot3Rect, Player.Instance.SpellSlotThree, _keySpell3Img);
    }

    private void useSpell(Image fillImg, RectTransform rect, Spell spell, Image keyImg) {
        fillImg.DOKill();
        rect.DOKill();
        rect.DOShakeRotation(0.1f * Settings.AnimationSpeed, 10, 10, 90, false);
        keyImg.DOKill();
        keyImg.DOFillAmount(1, 0.05f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            keyImg.DOFillAmount(0, spell.CooldownTime - 0.05f * Settings.AnimationSpeed).SetEase(Ease.Linear);
        });
        fillImg.DOFillAmount(1, 0.05f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            fillImg.DOFillAmount(0, spell.CooldownTime - 0.05f * Settings.AnimationSpeed).SetEase(Ease.Linear).OnComplete(() => {
                rect.DOShakeRotation(0.1f * Settings.AnimationSpeed, 10, 10, 90, false);
                rect.DOScale(Vector3.one * 1.05f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
                    rect.DOScale(Vector3.one, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutQuint);
                    rect.localRotation = Quaternion.identity;
                });
            });
        });

    }


    public void ShowMoreInfo(EffectUI effectUI) {
        _moreInfoRect.DOKill();
        LivingEntity.EffectData effectData = effectUI.EffectData;

        _moreInfoRect.localPosition = new Vector3(effectUI.transform.position.x - 235, _moreInfoRect.localPosition.y, _moreInfoRect.localPosition.z);
        _moreInfoRect.DOScale(1, 0.3f * Settings.AnimationSpeed).SetEase(Ease.OutQuint);

        _nameTextLocalizer.Key = effectData.Effect.Name;
        
        if (effectData.Effect.Modifiers.Length <= 0) return;

        _descriptionText.text = "";
        foreach (Modifier m in effectData.Effect.Modifiers) {
            _descriptionText.text += m.ToString() + "\n";
        }
    }

    public void HideMoreInfo() {
        _moreInfoRect.DOKill();
        _moreInfoRect.DOScale(0, 0.7f * Settings.AnimationSpeed).SetEase(Ease.OutQuint);
    }

    public void SetDuationText(float val) {
        if (float.IsInfinity(val)) {
            _durationText.text = "";
            _infinityGO.SetActive(true);
        } else {
            _infinityGO.SetActive(false);
            _durationText.text = $"{val:F1}s";
        }
    }


    public void OnUpdateConsumables() {
        updateConsumableOne();
        updateConsumableTwo();
    }

    private void updateConsumableOne() {
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
    }

    private void updateConsumableTwo() {
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
        
        _keyConsumable1Img.DOKill();
        _keyConsumable2Img.DOKill();
        _keyConsumable1Img.DOFillAmount(1, 0.05f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            _keyConsumable1Img.DOFillAmount(0, Player.Instance.ConsumableCooldown.CooldownTime - 0.05f * Settings.AnimationSpeed).SetEase(Ease.Linear);
        });
        _consumable1CooldownImg.DOFillAmount(1, 0.05f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            _consumable1CooldownImg.DOFillAmount(0, Player.Instance.ConsumableCooldown.CooldownTime - 0.05f * Settings.AnimationSpeed).SetEase(Ease.Linear);
        });
        _keyConsumable2Img.DOFillAmount(1, 0.05f * Settings.AnimationSpeed).SetEase(Ease.OutQuint).OnComplete(() => {
            _keyConsumable2Img.DOFillAmount(0, Player.Instance.ConsumableCooldown.CooldownTime - 0.05f * Settings.AnimationSpeed).SetEase(Ease.Linear);
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

    public void ShowBossBar(LivingEntity bossEntity) {
        _bossEntity = bossEntity;
        _bossBarImg.gameObject.SetActive(true);
        _bossBarGO.SetActive(true);
        _bossNameTextLocalizer.Key = bossEntity.DisplayName;
        _bossBarImg.fillAmount = bossEntity.Health / bossEntity.MaxHealth;
        _previousBossHealth = bossEntity.Health;

        _bossEntity.OnDamageTaken.AddListener(updateBossBar);
        _bossEntity.OnDeath.AddListener(() => {
            Debug.Log("Boss dead");
            _bossBarGO.SetActive(false);
            _bossEntity.OnDeath.RemoveAllListeners();
            _bossEntity.OnDamageTaken.RemoveListener(updateBossBar);
        });
    }

    private void updateBossBar(DamageTakenEventData _) {
        if (_bossEntity == null) return;
        _bossBarImg.DOKill();
        Debug.Log($"Boss health: {_bossEntity.Health}");
        _bossBarImg.DOFillAmount(_bossEntity.Health / _bossEntity.MaxHealth, 0.3f * Settings.AnimationSpeed).SetEase(Ease.OutCubic);

        _previousBossHealth = _bossEntity.Health;
    }

    public void HideBossBar() {
        _bossBarGO.SetActive(false);
        _bossEntity.OnDamageTaken.RemoveListener(updateBossBar);
        _bossEntity.OnDeath.RemoveAllListeners();
    }
}
