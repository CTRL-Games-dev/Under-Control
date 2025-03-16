using UnityEngine;
using UnityEngine.UI;

public class PlayerBarsHolder : MonoBehaviour

{
    [SerializeField] private Image _healthBarImg;
    [SerializeField] private Image _manaBarImg;

    private LivingEntity _livingEntity { get => UICanvas.Instance.PlayerLivingEntity; }

    private void Start() {
        UICanvas.Instance.PlayerLivingEntity.OnDamageTaken.AddListener(UpdateHealthBar);
    }

    private void UpdateHealthBar(DamageTakenEventData data) {
        _healthBarImg.fillAmount = _livingEntity.Health / _livingEntity.MaxHealth;
    }
}
