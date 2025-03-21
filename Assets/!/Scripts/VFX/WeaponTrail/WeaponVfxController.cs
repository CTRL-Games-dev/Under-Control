using UnityEngine;

using Unity.Behavior;
public class WeaponVfxController : MonoBehaviour {
    public ParticleSystem ParticleSystem;

    public void StartTrail() {
        ParticleSystem.Play();
    }

    public void StopTrail() {
        ParticleSystem.Stop();
    }
}
