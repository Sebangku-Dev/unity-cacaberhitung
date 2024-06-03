using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultipleDraggableSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] DraggableSlot[] draggableSlots;

    private void Awake()
    {
        Draggable.OnItemBeginDrag += UnlockDraggableRaycast;
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
        Draggable.OnItemBeginDrag -= UnlockDraggableRaycast;
    }

    private void UnlockDraggableRaycast()
    {
        foreach (var draggableSlot in draggableSlots)
        {
            draggableSlot.transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
        }
    }
}
