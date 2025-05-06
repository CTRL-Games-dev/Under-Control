using UnityEngine;

public class CandleLight : MonoBehaviour
{
    [SerializeField] private MeshRenderer _lightMeshRenderer;
    [SerializeField] private Color _lightColor1, _lightColor2;
    [SerializeField] private float _changeSpeed = 0.1f;
    [SerializeField] private float _moveSpeed = 10f;
    private Material _lightMaterial;
    private float _goalIntensity = 1.0f;
    private float _colorChange = 0;

    private void Awake() {
        _lightMaterial = _lightMeshRenderer.materials[1];
    }

    private void Start() {
        InvokeRepeating(nameof(setGoalIntensity), 0f, _changeSpeed);
    }

    void FixedUpdate() {
        float intensity = Mathf.MoveTowards(_lightMaterial.GetFloat("_Intensity"), _goalIntensity, _moveSpeed * Time.deltaTime);
        _lightMaterial.SetFloat("_Intensity", intensity);

        _colorChange = Mathf.PingPong(Time.time, 1f);
        _lightMaterial.SetColor("_EmissionColor", Color.Lerp(_lightColor1, _lightColor2, _colorChange));
    }

    private void setGoalIntensity() {
        _goalIntensity = Random.Range(1.2f, 5f);
    }
}
