using UnityEngine;

public class RotateOverLifetime : MonoBehaviour
{
    public float rotationSpeed = 360f;
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
