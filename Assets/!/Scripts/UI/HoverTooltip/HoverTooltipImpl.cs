using UnityEngine;

public abstract class HoverTooltipImpl<T> : MonoBehaviour where T : MonoBehaviour  {
    private T TrackedObject;

    protected abstract void UpdateTooltip(T component);

    void Update() {
        if(TrackedObject == null) {
            return;
        }

        UpdateTooltip(TrackedObject);
    }

    public void Enable(T trackedObject) {
        gameObject.SetActive(true);
        TrackedObject = trackedObject;
        UpdateTooltip(TrackedObject);
    }

    public void Disable() {
        TrackedObject = null;
        gameObject.SetActive(false);
    }
}