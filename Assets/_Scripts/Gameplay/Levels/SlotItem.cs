using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SlotItem : MonoBehaviour, IDropHandler
{

    public UnityEvent OnDropItem;
    public void OnDrop(PointerEventData eventData)
    {
            GameObject dropped = eventData.pointerDrag;
            Draggable draggableItem = dropped.GetComponent<Draggable>();
            draggableItem.parentAfterDrag = transform;

            OnDropItem?.Invoke();
    }
}
