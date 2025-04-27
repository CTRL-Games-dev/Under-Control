using UnityEngine;

public class BoneRotation : MonoBehaviour
{
    [SerializeField] private Transform _bone;
    void Start()
    {
        _bone.eulerAngles = new(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(0f, 359f), UnityEngine.Random.Range(-5f, 5f));
        _bone.position += new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0, UnityEngine.Random.Range(-0.1f, 0.1f));
    }
}
