using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultipleDraggableSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] public DraggableSlot[] draggableSlots;

    [SerializeField] private UnityEvent OnDropItem;

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

        OnDropItem?.Invoke();
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
