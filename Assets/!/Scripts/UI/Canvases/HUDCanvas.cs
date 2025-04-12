using TMPro;
using UnityEngine;
using UnityEngine.UI;

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


    private void Start() {
        Player.LivingEntity.Health.OnValueChanged.AddListener(UpdateHealthBar);
        Player.Instance.Mana.OnValueChanged.AddListener(UpdateManaBar);
    }

    private void UpdateHealthBar() {
        _healthBarImg.fillAmount = Player.LivingEntity.Health.Raw / Player.LivingEntity.MaxHealth.Raw;
        _healthText.text = $"{(int)Player.LivingEntity.Health.Raw}/{(int)Player.LivingEntity.MaxHealth.Raw}";
    }

    private void UpdateManaBar() {
        _manaBarImg.fillAmount = Player.Instance.Mana.Raw / Player.Instance.MaxMana.Raw;
        _manaText.text = $"{(int)Player.Instance.Mana.Raw}/{(int)Player.Instance.MaxMana.Raw}";
    }
}
