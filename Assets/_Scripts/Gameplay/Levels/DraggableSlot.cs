
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private bool LockAtFirst;
    [SerializeField] private int maximumChildCount = 1;
    [SerializeField] public Draggable relatedDraggable;

    private bool isLocked = false;

    [SerializeField] public bool isDisabled;

    public UnityEvent OnDropItem;

    private void Start()
    {
        if (LockAtFirst)
            Lock();
        else
            Unlock();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(isDisabled) return;

        if (!isLocked)
        {
            if (relatedDraggable != null && !(relatedDraggable == eventData.pointerDrag.gameObject?.GetComponent<Draggable>())) return;

            // One slot for one child
            if (transform.childCount <= maximumChildCount)
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
    }

    public void Unlock()
    {
        isLocked = false;
        gameObject.GetComponent<Image>().raycastTarget = true;
    }

    public void Lock()
    {
        isLocked = true;
        gameObject.GetComponent<Image>().raycastTarget = false;
    }

    protected bool IsLocked() => !GetComponent<Image>().raycastTarget;
}
