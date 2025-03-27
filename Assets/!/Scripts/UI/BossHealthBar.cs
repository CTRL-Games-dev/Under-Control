using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private LivingEntity _boss;

    private void Awake() {
        _boss.OnDamageTaken.AddListener(OnDamageTaken);
    }

    private void OnDamageTaken(DamageTakenEventData data) {
        _slider.value = _boss.Health / _boss.MaxHealth;
    }
}
