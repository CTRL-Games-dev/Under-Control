using UnityEngine;
using UnityEngine.UI;

public class PlayerBarsHolder : MonoBehaviour

{
    [SerializeField] Image _healthBarImg;
    [SerializeField] Image _manaBarImg;
    [SerializeField] Image _controlBarImg;


    public GameObject Player;
    public PlayerController PlayerController;
    public LivingEntity PlayerEntity;


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


    private void Start() {
    }


    private void Update() {
        // Temporary solution to update bars

        HealthBarFillAmount = PlayerEntity.Health / PlayerEntity.MaxHealth;
        // ManaBarFillAmount = _manaBarFillAmount;
        // ControlBarFillAmount = _controlBarFillAmount;
    }
}
