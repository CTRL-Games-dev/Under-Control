using UnityEngine;

public class Pinecone : MonoBehaviour {
    public float Speed = 5;
    public float MinDamage = 30;
    public float MaxDamage = 60;
    public float Range = 40;

    private Vector3 _direction;
    private LivingEntity _caster;
    private Vector3 _startingPosition;

    public void Initialize(LivingEntity caster, Vector3 direction) {
        _caster = caster;
        _direction = direction;
    }

    void Awake() {
        _startingPosition = transform.position;
    }

    private void Update() {
        transform.position += _direction * Speed * Time.deltaTime;

        if(Vector3.Distance(transform.position, _startingPosition) > Range) {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other) {
        LivingEntity entity = other.GetComponentInParent<LivingEntity>();
        
        if(entity == null) return;
        if(entity == _caster) return;
        if(!_caster.Guild.IsHostileTowards(entity.Guild)) return;

        entity.TakeDamage(new Damage {
            Type = DamageType.PHYSICAL,
            Value = Random.Range(MinDamage, MaxDamage)
        });

        if(entity.IsPlayer) {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.RockHit, this.transform.position);
            CameraManager.ShakeCamera(7, 0.35f);
        }

        Destroy(gameObject);
    }
}