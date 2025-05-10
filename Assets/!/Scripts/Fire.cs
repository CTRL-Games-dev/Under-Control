using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Fire : MonoBehaviour {
    public static List<float> DamageOverStacks = new List<float> {
        5,
        7,
        8,
    };

    public int Stacks = 0;

    private LivingEntity _self;
    private VisualEffect _fireVFX;

    void Start() {
        _self = GetComponent<LivingEntity>();
        
        _self.TintAnimator.SetTint(Color.red, 0.5f);

        _fireVFX = Instantiate(GameManager.Instance.FireEffectPrefab, gameObject.transform);

        _fireVFX.Play();
    }

    public void Stack() {
        Stacks++;
    }

    public void Unstack() {
        Stacks--;

        if(Stacks <= 0) {
            _self.TintAnimator.ResetTint();
            Destroy(_fireVFX.gameObject);
            Destroy(this);
        }
    }

    void Update() {
        float currentDamage;
        if(Stacks > DamageOverStacks.Count) {
            currentDamage = DamageOverStacks[DamageOverStacks.Count - 1];
        } else {
            currentDamage = DamageOverStacks[Stacks - 1];
        }

        _self.Health -= currentDamage * Time.deltaTime;
    }
}