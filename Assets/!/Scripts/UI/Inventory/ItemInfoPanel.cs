using TMPro;
using UnityEngine;

public class ItemInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject _lightAttackHolder, _heavyAttackHolder;
    [SerializeField] private TextMeshProUGUI _itemLightDamage, _itemHeavyDamage, _itemValue, _itemAmount;
    [SerializeField] private TextLocalizer _nameTextLocalizer, _rarityTextLocalizer, _descriptionTextLocalizer;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    
    private void Update() {
        Vector2 scale = new(Screen.width / 1920f, Screen.height / 1080f);

        transform.position = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _rectTransform.rect.width * scale.x), 
            Mathf.Clamp(Input.mousePosition.y, _rectTransform.rect.height * scale.y, Screen.height)
        );
    }

    public void ShowItemInfo(ItemUI itemUI) {
        if (gameObject.activeSelf) _canvasGroup.alpha = 0;

        gameObject.SetActive(itemUI != null);
        
        if (itemUI == null) return;
        
        _canvasGroup.alpha = 0;

        InventoryItem item = itemUI.InventoryItem;

        

        transform.position = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - _rectTransform.rect.width), 
            Mathf.Clamp(Input.mousePosition.y, _rectTransform.rect.height, Screen.height)
        );
        _canvasGroup.alpha = 1;

        _nameTextLocalizer.Key = item.ItemData.DisplayName;
        _descriptionTextLocalizer.Key = item.ItemData.Description;

        if (itemUI.InventoryItem.ItemData.GetType() == typeof(WeaponItemData)) {
            WeaponItemData weaponItemData = (WeaponItemData)item.ItemData;
            _lightAttackHolder.SetActive(true);
            _heavyAttackHolder.SetActive(true);
            _itemLightDamage.text = $"{weaponItemData.LightDamageMin} - {weaponItemData.LightDamageMax}";
            _itemHeavyDamage.text = $"{weaponItemData.HeavyDamageMin} - {weaponItemData.HeavyDamageMax}";
        } else {
            _lightAttackHolder.SetActive(false);
            _heavyAttackHolder.SetActive(false);
        }

        int value = item.ItemData.Value / 2;
        if (itemUI.CurrentInventoryPanel != null && itemUI.CurrentInventoryPanel.IsSellerInventory) value *= 2;

        _itemValue.text = value + $" ({value * item.Amount})";
        _itemAmount.text = '×' + item.Amount.ToString();
    }
}
