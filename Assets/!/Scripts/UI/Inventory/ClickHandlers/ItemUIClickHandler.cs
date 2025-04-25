using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUIClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ItemUI _itemUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioClip InvClickClip = Resources.Load("SFX/click") as AudioClip;
        SoundFXManager.Instance.PlaySoundFXClip(InvClickClip,transform);
        if (eventData.button == PointerEventData.InputButton.Left) {
            EventBus.ItemUILeftClickEvent.Invoke(_itemUI);
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            EventBus.ItemUIRightClickEvent.Invoke(_itemUI);
        }
    }
}
