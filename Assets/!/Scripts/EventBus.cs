using UnityEngine;
using UnityEngine.Events;

public static class EventBus {
    public static UnityEvent OnInventoryItemChanged = new UnityEvent();
}
