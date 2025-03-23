using UnityEngine;

using Unity.Behavior;
public class WeaponVfxController : MonoBehaviour {
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private GameObject _hitTrail;


    public void StartTrail() {
        _hitTrail.SetActive(true);
        // _hitParticles.Play();0
    }

    public void StopTrail() {
        _hitTrail.SetActive(false);
        // _hitParticles.Stop();
    }
}
