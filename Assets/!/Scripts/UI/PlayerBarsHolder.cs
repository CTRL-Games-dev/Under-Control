using UnityEngine;
using UnityEngine.UI;

public class PlayerBarsHolder : MonoBehaviour

{
    [SerializeField] private Image _healthBarImg;
    [SerializeField] private Image _manaBarImg;
    [SerializeField] private Image _controlBarImg;

    private UICanvas _uiCanvas;
    private LivingEntity _livingEntity { get => _uiCanvas.PlayerLivingEntity; }

    private void Start() {
        _uiCanvas = UICanvas.Instance;   
    }

    private void Update() {
        // Temporary solution to update bars
        _healthBarImg.fillAmount = _livingEntity.Health / _livingEntity.MaxHealth;
        _manaBarImg.fillAmount = 1;
        _controlBarImg.fillAmount = 1;
    }
}
