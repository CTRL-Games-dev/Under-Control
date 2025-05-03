using UnityEngine;

public class IceShard : MonoBehaviour {
    public Effect FreezeEffect;
    public float Range;
    public float Speed;

    private Vector3 _startingPosition;
    private LivingEntity _caster;
    private Vector3 _direction;

    public void Initialize(LivingEntity caster, Vector3 direction) {
        _caster = caster;
        _direction = direction;
    }

    public void Awake() {
        _startingPosition = transform.position;
    }

    void Update() {
        transform.position += _direction * Speed * Time.deltaTime;
        if(Vector3.Distance(transform.position, _startingPosition) > Range) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        if(!other.TryGetComponent(out LivingEntity livingEntity)) return;
        if(livingEntity == _caster) return;

        livingEntity.ApplyEffect(FreezeEffect);

        Destroy(gameObject);
        Debug.Log("");
    }
}