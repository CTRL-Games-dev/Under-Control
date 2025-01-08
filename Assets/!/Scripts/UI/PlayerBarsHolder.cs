using UnityEngine;
using UnityEngine.UI;

public class PlayerBarsHolder : MonoBehaviour

{
    [SerializeField] readonly Image _healthBarImg;
    [SerializeField] readonly Image _manaBarImg;
    [SerializeField] readonly Image _controlBarImg;


    [SerializeField] float _healthBarFillAmount;
    [SerializeField] float _manaBarFillAmount;
    [SerializeField] float _controlBarFillAmount;


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
        HealthBarFillAmount = _healthBarFillAmount;
        ManaBarFillAmount = _manaBarFillAmount;
        ControlBarFillAmount = _controlBarFillAmount;
    }


    private void Update() {
        // Temporary solution to update bars

        HealthBarFillAmount = _healthBarFillAmount;
        ManaBarFillAmount = _manaBarFillAmount;
        ControlBarFillAmount = _controlBarFillAmount;
    }
}
