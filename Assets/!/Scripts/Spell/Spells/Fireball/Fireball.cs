using UnityEngine;

public class Fireball : MonoBehaviour {
    public Effect FireEffect;
    public float Range;
    public float Speed;
    public float ExplosionRadius;
    public float ExplosionDamage;

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
            Explode();
            Destroy(gameObject);
        }
    }

    void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach (Collider collider in colliders) {
            if(collider.gameObject.TryGetComponent(out LivingEntity entity) && entity != _caster) {
                entity.ApplyEffect(FireEffect);
                entity.TakeDamage(new Damage{Value = ExplosionDamage, Type =  DamageType.MAGICAL}, _caster);
            }
        }
        Destroy(gameObject);
    }
    void OnTriggerEnter(Collider other) {
        if(!other.TryGetComponent(out LivingEntity livingEntity)) return;
        if(livingEntity == _caster) return;
        Explode();
    }
}