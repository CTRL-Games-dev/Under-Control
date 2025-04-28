using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ItemEntityTooltip : HoverTooltipImpl<ItemEntity> {
    public Image ItemIconImage;
    public TextLocalizer ItemNameTextLocalizer;
    public TextMeshProUGUI ItemAmountText;
    public RectTransform RectTransform;

    protected override void UpdateTooltip(ItemEntity itemEntity) {
        ItemIconImage.sprite = itemEntity.ItemData.Icon;
        ItemNameTextLocalizer.Key = itemEntity.ItemData.DisplayName;
        ItemAmountText.text = itemEntity.Amount.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
    }
}