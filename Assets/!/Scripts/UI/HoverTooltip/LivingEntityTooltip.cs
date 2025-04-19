using UnityEngine.UI;

public class LivingEntityTooltip : HoverTooltipImpl<LivingEntity> {
    public TextLocalizer DisplayNameTextLocalizer;
    public Slider HealthBarSider;

    protected override void UpdateTooltip(LivingEntity livingEntity) {
        DisplayNameTextLocalizer.Key = livingEntity.DisplayName;
        HealthBarSider.value = livingEntity.Health / livingEntity.MaxHealth; 
    }
}