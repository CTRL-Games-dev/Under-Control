using TMPro;
using UnityEngine.UI;

public class LivingEntityTooltip : HoverTooltipImpl<LivingEntity> {
    public TextMeshProUGUI DisplayNameText;
    public TextMeshProUGUI LevelText;
    public Image HealthBarImage;

    protected override void UpdateTooltip(LivingEntity livingEntity) {
        DisplayNameText.text = livingEntity.DisplayName;
        LevelText.text = livingEntity.Level.ToString();
        HealthBarImage.fillAmount = livingEntity.Health / livingEntity.MaxHealth; 
    }
}