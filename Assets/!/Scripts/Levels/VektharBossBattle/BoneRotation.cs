using UnityEngine;

public class BoneRotation : MonoBehaviour
{
    [SerializeField] private Transform _bone;
    void Start()
    {
        _bone.eulerAngles = new(Random.Range(-5f, 5f), Random.Range(0f, 359f), Random.Range(-5f, 5f));
        _bone.position += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.1f, 0.1f));
    }
}
