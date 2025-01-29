using UnityEngine.Events;

public static class EventBus {
    public static UnityEvent OnInventoryItemChanged = new();
    public static UnityEvent<ItemUI> OnItemUIClick = new();
    public static UnityEvent<InvTile> OnInvTileClick = new();
    public static UnityEvent<InventoryItem> OnItemUIHover = new(); 

    public static UnityEvent OnInventoryOpen = new UnityEvent();
    public static UnityEvent OnInventoryClose = new UnityEvent();
}
