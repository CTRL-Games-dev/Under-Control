using UnityEngine;

public class WeaponsVfxController : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    void Start()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>(); 
    }

    public void StartTrail()
    {
        _particleSystem.Play();
    }

    public void StopTrail()
    {
        _particleSystem.Stop();
    }
}
