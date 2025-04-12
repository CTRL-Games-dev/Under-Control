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


    private void Update() {
        UpdateHealthBar();
        UpdateManaBar();
    }

    private void UpdateHealthBar() {
        _healthBarImg.fillAmount = Player.LivingEntity.Health / Player.LivingEntity.MaxHealth;
        _healthText.text = $"{(int)Player.LivingEntity.Health}/{(int)Player.LivingEntity.MaxHealth}";
    }

    private void UpdateManaBar() {
        _manaBarImg.fillAmount = Player.Instance.Mana / Player.Instance.MaxMana;
        _manaText.text = $"{(int)Player.Instance.Mana}/{(int)Player.Instance.MaxMana}";
    }
}
