using UnityEngine;

public class VfxController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    public bool IsEmitting; 

    void Start()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();  
    }

    void Update()
    {
        if (IsEmitting)
        {
            if (!_particleSystem.isEmitting)
            {
                _particleSystem.Play();
            }
        }
        else
        {
            if (_particleSystem.isEmitting)
            {
                _particleSystem.Stop();
            }
        }
    }

    public void ToggleTrail(bool toggle)
    {
        IsEmitting = toggle;
    }
}
