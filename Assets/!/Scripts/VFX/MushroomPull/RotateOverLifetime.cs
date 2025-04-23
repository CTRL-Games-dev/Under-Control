using UnityEngine;

public class RotateOverLifetime : MonoBehaviour
{
    public float rotationSpeed = 45f; // Degrees per second
    void Update()
    {
        // Rotate the object around its Y-axis at a speed of 45 degrees per second
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
