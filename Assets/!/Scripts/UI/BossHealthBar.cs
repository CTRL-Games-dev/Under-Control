using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LivingEntity))]
public class BossHealthBar : MonoBehaviour {
    [SerializeField] private Slider _slider;
    private LivingEntity _livingEntity;

    private void Awake() {
        _livingEntity = GetComponent<LivingEntity>();

        _livingEntity.OnDamageTaken.AddListener(OnDamageTaken);
    }

    private void OnDamageTaken(DamageTakenEventData data) {
        _slider.value = _livingEntity.Health / _livingEntity.MaxHealth;
    }
}
