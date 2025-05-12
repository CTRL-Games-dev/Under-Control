using UnityEngine;

public class Freeze : MonoBehaviour {
    public int Stacks = 0;

    private LivingEntity _self;
    private TintAnimator.Tint _freezeTint;

    void Start() {
        _self = GetComponent<LivingEntity>();
        
        _freezeTint = _self.TintAnimator.ApplyTint(Color.cyan, 0.8f, 100000);
    }

    public void Stack() {
        Stacks++;
    }

    public void Unstack() {
        Stacks--;

        if(Stacks <= 0) {
            _freezeTint.Stop();
            Destroy(this);
        }
    }
}