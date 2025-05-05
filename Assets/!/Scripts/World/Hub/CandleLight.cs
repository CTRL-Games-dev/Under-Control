using UnityEngine;

public class CandleLight : MonoBehaviour
{
    [SerializeField] private Material _lightMaterial;
    [SerializeField] private float _lightIntensity = 1.0f;
    [SerializeField] private float _lightRange = 5.0f;

    void Update() {

        float flicker = Mathf.PingPong(Time.time * _lightIntensity, 1.0f);
        _lightMaterial.SetFloat("_LightIntensity", flicker);
        _lightMaterial.SetColor("_LightColor", Color.Lerp(Color.yellow, Color.white, flicker));
        _lightMaterial.SetFloat("_LightRange", _lightRange);
    }
}
