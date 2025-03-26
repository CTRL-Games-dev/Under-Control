using UnityEngine;
using UnityEngine.UI;

public class PlayerBarsHolder : MonoBehaviour

{
    [SerializeField] private Image _healthBarImg;
    [SerializeField] private Image _manaBarImg;


    private void Start() {
        Player.LivingEntity.OnDamageTaken.AddListener(UpdateHealthBar);
    }

    private void UpdateHealthBar(DamageTakenEventData data) {
        _healthBarImg.fillAmount = Player.LivingEntity.Health / Player.LivingEntity.MaxHealth;
    }
}
