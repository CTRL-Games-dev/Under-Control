using UnityEngine;
using UnityEngine.Events;

public class MushroomPullComponent : MonoBehaviour {
    public GameObject Puller;
    public float PullSpeed = 5f;
    public float StoppingDistance = 2f; 
    public UnityEvent OnPulled = new();

    void FixedUpdate() {
        if(Puller == null) Destroy(this);

        Vector3 pullerPosition = Puller.transform.position;
        Vector3 selfPosition = transform.position;

        float distance = Vector3.Distance(pullerPosition, selfPosition);
        if(distance < StoppingDistance) {
            OnPulled?.Invoke();
            Destroy(this);
        }

        transform.position = Vector3.MoveTowards(transform.position, pullerPosition, PullSpeed * Time.fixedDeltaTime);
    }
}