using UnityEngine;

public class Charm : MonoBehaviour {
    public Guild PreviousGuild;

    private LivingEntity _self;

    void Awake() {
        _self = GetComponent<LivingEntity>();
    }

    public void Initialize(float duration) {
        PreviousGuild = _self.Guild;
        _self.Guild = Player.LivingEntity.Guild;
        
        Invoke(nameof(Stop), duration);
    }

    public void Stop() {
        _self.Guild = PreviousGuild;
        _self.TintAnimator.ResetTint();
        Destroy(this);
    }
}