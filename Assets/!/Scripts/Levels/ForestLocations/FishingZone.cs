using UnityEngine;

public class FishingZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Player.Instance.CanFish = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) Player.Instance.CanFish = false;
    }
}
