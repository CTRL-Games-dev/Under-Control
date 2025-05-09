using UnityEngine;
using UnityEngine.Events;

public class SlashEdge : MonoBehaviour
{
    [SerializeField] private Transform _edgeToFollow;
    [SerializeField] private Damage SlashDamage;
    public Vector3 Position => _edgeToFollow.position;

    public UnityEvent<LivingEntity> OnHit = new();
    public bool HitboxEnabled = true;
    private SlashManager _slashManager;

    void Awake() {
        _slashManager = GetComponentInParent<SlashManager>();
    }

    void Update() {
        if (_edgeToFollow != null){
            transform.position = new Vector3(_edgeToFollow.position.x, 0.5f, _edgeToFollow.position.z);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) return;
        if (_slashManager.HitEnemies.Contains(other)) return;

        LivingEntity victim = other.GetComponentInParent<LivingEntity>(includeInactive: true);

        if(victim == null) return;
        _slashManager.HitEnemies.Add(other);

        victim.TakeDamage(SlashDamage, null);
    }


}
