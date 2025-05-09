using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class DeadZone : MonoBehaviour
{
    private Collider _collider;
    void Awake() {
        _collider = GetComponent<Collider>();

    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("Something enterred dead zone");
        if(other.GetComponent<Player>() == null) {
            return;
        }
        Debug.Log("Player enterred dead zone");

        AdventureManager.ReturnPlayerToStart();
    }
}