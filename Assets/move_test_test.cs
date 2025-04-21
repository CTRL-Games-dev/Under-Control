using UnityEngine;

public class move_test_test : MonoBehaviour
{
    public bool Move = false;
    [SerializeField] private float speed = 5f;
    void Update()
    {
        if (Move) {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
    }
}
