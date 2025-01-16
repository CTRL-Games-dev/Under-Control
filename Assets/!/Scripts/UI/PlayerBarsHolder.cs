using UnityEngine;
using UnityEngine.UI;

public class PlayerBarsHolder : MonoBehaviour

{
    [SerializeField] private Image _healthBarImg;
    [SerializeField] private Image _manaBarImg;
    [SerializeField] private Image _controlBarImg;


    private UICanvas _uiCanvasParent;
    private GameObject _player;
    private LivingEntity _livingEntity;
    private float _maxHealth;



    public float HealthBarFillAmount { set { 
        _healthBarImg.fillAmount = value; 
        } 
    }
    public float ManaBarFillAmount { set { 
        _manaBarImg.fillAmount = value; 
        } 
    }
    public float ControlBarFillAmount { set {
        _controlBarImg.fillAmount = value; 
        } 
    }

    private void Awake() {
        _uiCanvasParent = gameObject.GetComponentInParent<UICanvas>();
    }

    private void Start() {
        _livingEntity = _uiCanvasParent.PlayerLivingEntity;
        // _livingEntity = _player.GetComponent<PlayerController>().LivingEntity;
        _maxHealth = _livingEntity.MaxHealth;
    }


    private void Update() {
        // Temporary solution to update bars

        // HealthBarFillAmount = _livingEntity.Health / _maxHealth;
        // ManaBarFillAmount = _manaBarFillAmount;
        // ControlBarFillAmount = _controlBarFillAmount;
    }
}
