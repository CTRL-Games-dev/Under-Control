using UnityEngine;

public class SlashEdge : MonoBehaviour
{
    [SerializeField] private Transform _edgeToFollow;
    public Vector3 Position => _edgeToFollow.position;
    PolygonCollider2D _polygonCollider2D;


    void Update() {
        if (_edgeToFollow != null){
            transform.SetPositionAndRotation(_edgeToFollow.position, _edgeToFollow.rotation);

        }
    }


}
