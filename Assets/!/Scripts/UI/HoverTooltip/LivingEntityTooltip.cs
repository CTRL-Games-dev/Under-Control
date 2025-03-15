using TMPro;
using UnityEngine.UI;

public class LivingEntityTooltip : HoverTooltipImpl<LivingEntity> {
    public TextMeshProUGUI DisplayNameText;
    public TextMeshProUGUI LevelText;
    public Slider HealthBarSider;

    protected override void UpdateTooltip(LivingEntity livingEntity) {
        DisplayNameText.text = livingEntity.DisplayName;
        LevelText.text = livingEntity.Level.ToString();
        HealthBarSider.value = livingEntity.Health / livingEntity.MaxHealth; 
    }
}