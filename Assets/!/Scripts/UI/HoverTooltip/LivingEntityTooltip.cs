using TMPro;
using UnityEngine.UI;

public class LivingEntityTooltip : HoverTooltipImpl<LivingEntity> {
    public TextLocalizer DisplayNameTextLocalizer;
    public TextMeshProUGUI LevelText;
    public Slider HealthBarSider;

    protected override void UpdateTooltip(LivingEntity livingEntity) {
        DisplayNameTextLocalizer.Key = livingEntity.DisplayName;
        LevelText.text = livingEntity.Level.ToString();
        HealthBarSider.value = livingEntity.Health / livingEntity.MaxHealth; 
    }
}