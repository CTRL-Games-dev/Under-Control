using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    public void SetHealth(float val) {
        _slider.value = val;
    }
}
