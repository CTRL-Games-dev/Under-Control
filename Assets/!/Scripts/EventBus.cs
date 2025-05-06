using UnityEngine.Events;

public static class EventBus {
    public static UnityEvent InventoryItemChangedEvent = new();
    public static UnityEvent ItemPlacedEvent = new();
    public static UnityEvent<ItemUI> ItemUILeftClickEvent = new();
    public static UnityEvent<ItemUI> ItemUIRightClickEvent = new();
    public static UnityEvent<ItemUI> ItemUIHoverEvent = new(); 
    public static UnityEvent TileSizeSetEvent = new();

    public static UnityEvent InventoryClosedEvent = new();
    public static UnityEvent<Card> RunCardClickedEvent = new();

    public static UnityEvent SceneReadyEvent = new();
}
