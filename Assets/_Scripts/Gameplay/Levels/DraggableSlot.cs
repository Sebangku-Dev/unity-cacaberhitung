using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DraggableSlot : MonoBehaviour, IDropHandler
{
    public UnityEvent OnDropItem;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        Draggable draggableItem = dropped.GetComponent<Draggable>();

        // Cancel drag and drop if draggable is locked
        if (draggableItem.isLocked) return;

        if (!draggableItem.isNotSnapped)
            draggableItem.parentAfterDrag = transform;

        if (draggableItem.parentBeforeDrag != transform)
            OnDropItem?.Invoke();
    }
}
