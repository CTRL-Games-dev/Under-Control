using TMPro;
using UnityEngine;

public class ItemInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject _lightAttackHolder, _heavyAttackHolder;
    [SerializeField] private TextMeshProUGUI _itemLightDamage, _itemHeavyDamage, _itemValue, _itemAmount;
    [SerializeField] private TextLocalizer _nameTextLocalizer, _rarityTextLocalizer, _descriptionTextLocalizer;

    private RectTransform _rectTransform;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    private void Update() {
        transform.position = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _rectTransform.rect.width * UICanvas.ScreenScale.x), 
            Mathf.Clamp(Input.mousePosition.y, _rectTransform.rect.height * UICanvas.ScreenScale.y, Screen.height)
        );
    }

    public void ShowItemInfo(ItemUI itemUI) {
        gameObject.SetActive(itemUI != null);
        
        if (itemUI == null) return;

        transform.position = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _rectTransform.rect.width * UICanvas.ScreenScale.x), 
            Mathf.Clamp(Input.mousePosition.y, _rectTransform.rect.height * UICanvas.ScreenScale.y, Screen.height)
        );

        InventoryItem item = itemUI.InventoryItem;

        _nameTextLocalizer.Key = item.ItemData.DisplayName;
        _descriptionTextLocalizer.Key = item.ItemData.Description;

        if (itemUI.InventoryItem.ItemData is WeaponItemData weaponItemData && weaponItemData.LightDamageMax > 0 && weaponItemData.HeavyDamageMax > 0) {
            _lightAttackHolder.SetActive(true);
            _heavyAttackHolder.SetActive(true);
            _itemLightDamage.text = $"{(int)(weaponItemData.LightDamageMin * item.PowerScale)} - {(int)(weaponItemData.LightDamageMax * item.PowerScale)}";
            _itemHeavyDamage.text = $"{(int)(weaponItemData.HeavyDamageMin * item.PowerScale)} - {(int)(weaponItemData.HeavyDamageMax * item.PowerScale)}";
        } else {
            _lightAttackHolder.SetActive(false);
            _heavyAttackHolder.SetActive(false);
        }

        int value = item.ScaledValue / 2;
        if (itemUI.CurrentInventoryPanel != null && itemUI.CurrentInventoryPanel.IsSellerInventory) {
            value *= 2;
        }

        _itemValue.text = value + $" ({value * item.Amount})";
        _itemAmount.text = '×' + item.Amount.ToString();
    }
}
