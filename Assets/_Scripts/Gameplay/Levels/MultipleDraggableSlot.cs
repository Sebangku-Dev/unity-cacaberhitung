using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultipleDraggableSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] DraggableSlot[] draggableSlots;

    private void Awake()
    {
        Draggable.OnItemBeginDrag += LockDraggableRaycast;
        Draggable.OnItemEndDrag += UnlockDraggableRaycast;
    }

    private void Start()
    {
        // Override draggable slot control in parent
        foreach (var draggableSlot in draggableSlots)
        {
            draggableSlot.Lock();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        foreach (var draggableSlot in draggableSlots)
        {
            if (eventData.pointerDrag.gameObject.GetComponent<Draggable>() == draggableSlot.relatedDraggable)
            {
                eventData.pointerDrag.gameObject.GetComponent<Draggable>().parentAfterDrag = draggableSlot.transform;
            }
        }
    }

    private void OnDestroy()
    {
        Draggable.OnItemBeginDrag -= LockDraggableRaycast;
        Draggable.OnItemEndDrag -= UnlockDraggableRaycast;
    }

    private void LockDraggableRaycast()
    {
        GetComponent<Image>().raycastTarget = true;
        foreach (var draggableSlot in draggableSlots)
        {
            if (draggableSlot.transform.childCount > 0)
            {
                draggableSlot.transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
            }
        }
    }
    private void UnlockDraggableRaycast()
    {
        GetComponent<Image>().raycastTarget = false;
        foreach (var draggableSlot in draggableSlots)
        {
            if (draggableSlot.transform.childCount > 0)
            {
                draggableSlot.transform.GetChild(0).GetComponent<Image>().raycastTarget = true;
            }
        }
    }
}
