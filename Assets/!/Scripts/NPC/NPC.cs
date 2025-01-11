using UnityEngine;
using UnityEngine.Events;

public abstract class NPC : MonoBehaviour, IInteractable
{
    public UnityEvent StartedInteraction;
    public UnityEvent StoppedInteraction;
    public abstract string GetInteractString();
    public abstract void StartInteraction();
    public abstract void StopInteraction();
}