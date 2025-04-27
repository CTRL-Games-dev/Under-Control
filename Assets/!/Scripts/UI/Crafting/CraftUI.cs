using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class CraftUI : MonoBehaviour
{
    [SerializeField] private Sprite _plusIcon, _equalsIcon;
    private RectTransform _rectTransform;
    [SerializeField] private GameObject _ingredientPrefab;
    [SerializeField] private Transform _imgHolder;
    [SerializeField] private Image _canCraftIndicator;
    private ConsumableItemData _consumableItemData;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Setup(ConsumableItemData consumableItemData) {
        EventBus.InventoryItemChangedEvent.AddListener(UpdateCanCraftIndicator);
        _consumableItemData = consumableItemData;
        int ingredientCount = consumableItemData.Ingredients.Count;

        for (int i = 0; i < ingredientCount; i++) {
            Image img = Instantiate(_ingredientPrefab, _imgHolder).GetComponent<Image>();
            img.sprite = consumableItemData.Ingredients[i].Icon;
            img.preserveAspect = true;
            if (i == ingredientCount -1) {
                Instantiate(_ingredientPrefab, _imgHolder).GetComponent<Image>().sprite = _equalsIcon;
            } else {
                Instantiate(_ingredientPrefab, _imgHolder).GetComponent<Image>().sprite = _plusIcon;
            }
        }

        Instantiate(_ingredientPrefab, _imgHolder).GetComponent<Image>().sprite = consumableItemData.Icon;
        SetCanCraftIndicator(getCanCraft(consumableItemData));
    }

    public void UpdateCanCraftIndicator() {
        SetCanCraftIndicator(getCanCraft(_consumableItemData));
    }

    public void OnPointerEnter() {
        _rectTransform.DOComplete();
        _rectTransform.DOKill();
        _rectTransform.DOScaleY(1.05f, 0.2f * Settings.AnimationSpeed);
    }

    public void OnPointerExit() {
        _rectTransform.DOComplete();
        _rectTransform.DOKill();
        _rectTransform.DOScaleY(1, 0.2f * Settings.AnimationSpeed);
    }

    public void OnPointerClick() {
        if (getCanCraft(_consumableItemData)) {
            if (Player.Inventory.AddItem(_consumableItemData, 1, 0f, false)) {
                for (int i = 0; i < _consumableItemData.Ingredients.Count; i++) {
                    InventoryItem inventoryItem = Player.Inventory.GetFirstInventoryItem(_consumableItemData.Ingredients[i]);
                    inventoryItem.Amount -= 1;
                    if (inventoryItem.Amount <= 0) {
                        Player.Inventory.RemoveInventoryItem(inventoryItem);
                    }
                }
                EventBus.InventoryItemChangedEvent?.Invoke();
                _rectTransform.DOScaleY(0.9f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutBounce).OnComplete(() => {
                    _rectTransform.DOScaleY(1f, 0.1f * Settings.AnimationSpeed).SetEase(Ease.OutBounce);
                });
            }
        }
    }

    public void SetCanCraftIndicator(bool canCraft) {
        _canCraftIndicator.DOComplete();
        _canCraftIndicator.DOKill();
        if (canCraft) {
            _canCraftIndicator.DOColor(Color.green, 0.2f * Settings.AnimationSpeed);
        } else {
            _canCraftIndicator.DOColor(Color.red, 0.2f * Settings.AnimationSpeed);
        }
    }

    private bool getCanCraft(ConsumableItemData consumableItemData) {
        List<ItemData> ingredients = consumableItemData.Ingredients;

        for (int i = 0; i < ingredients.Count; i++) {
            if (!Player.Inventory.HasItemData(ingredients[i])) {
                return false;
            }
        }

        return true;
    }
}
