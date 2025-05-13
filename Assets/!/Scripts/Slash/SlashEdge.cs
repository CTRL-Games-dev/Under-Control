using UnityEngine;

public class SlashEdge : MonoBehaviour
{
    public Vector3 Position => _edgeToFollow.position;

    [SerializeField] private Transform _edgeToFollow, _edgeToFollow2;
    [SerializeField] private Damage SlashDamage;

    [SerializeField] private bool _isInbetween = false;

    void Update() {
        if (_edgeToFollow != null){
            if (_isInbetween) {
                transform.SetPositionAndRotation(
                    new Vector3(
                    (_edgeToFollow.position.x + _edgeToFollow2.position.x) / 2,
                    0.5f,
                    (_edgeToFollow.position.z + _edgeToFollow2.position.z) / 2
                    ),
                    Quaternion.identity
                );
                    
            } else {
                transform.SetPositionAndRotation(new Vector3(_edgeToFollow.position.x, 0.5f, _edgeToFollow.position.z), Quaternion.identity);
            }

        }
    }
}
