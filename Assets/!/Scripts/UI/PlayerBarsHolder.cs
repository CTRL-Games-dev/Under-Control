using UnityEngine;
using UnityEngine.UI;

public class PlayerBarsHolder : MonoBehaviour

{
    [SerializeField] Image _healthBarImg;
    [SerializeField] Image _manaBarImg;
    [SerializeField] Image _controlBarImg;


    [SerializeField] float _healthBarFillAmount;
    [SerializeField] float _manaBarFillAmount;
    [SerializeField] float _controlBarFillAmount;


    public float HealthBarFillAmount { set { _healthBarImg.fillAmount = value; } }
    public float ManaBarFillAmount { set { _manaBarImg.fillAmount = value; } }
    public float ControlBarFillAmount { set { _controlBarImg.fillAmount = value; } }


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
