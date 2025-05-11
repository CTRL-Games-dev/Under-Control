using UnityEngine;

public class Charm : MonoBehaviour {
    public Guild PreviousGuild;

    private LivingEntity _self;
    private TintAnimator.Tint _tint;

    void Awake() {
        _self = GetComponent<LivingEntity>();
    }

    public void Initialize(float duration) {
        PreviousGuild = _self.Guild;
        _self.Guild = Player.LivingEntity.Guild;
        _tint = _self.TintAnimator.ApplyTint(Color.magenta, 0.5f, 100000);
        
        Invoke(nameof(Stop), duration);
    }

    public void Stop() {
        _self.Guild = PreviousGuild;
        _tint.Stop();
        Destroy(this);
    }
}