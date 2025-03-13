using UnityEngine.UI;
using TMPro;

public class ItemEntityTooltip : HoverTooltipImpl<ItemEntity> {
    public Image ItemIconImage;
    public TextMeshProUGUI ItemNameText;
    public TextMeshProUGUI ItemAmountText;

    protected override void UpdateTooltip(ItemEntity itemEntity) {
        ItemIconImage.sprite = itemEntity.ItemData.Icon;
        ItemNameText.text = itemEntity.ItemData.DisplayName;
        ItemAmountText.text = itemEntity.Amount.ToString();
    }
}