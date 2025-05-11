using UnityEngine;

public class SlashEdge : MonoBehaviour
{
    public Vector3 Position => _edgeToFollow.position;

    [SerializeField] private Transform _edgeToFollow;
    [SerializeField] private Damage SlashDamage;

    void Update() {
        if (_edgeToFollow != null){
            transform.position = new Vector3(_edgeToFollow.position.x, 0.5f, _edgeToFollow.position.z);
        }
    }
}
