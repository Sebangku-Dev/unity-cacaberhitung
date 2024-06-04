using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public bool isNotSnapped = false;
    [SerializeField] public bool isDestroyable = false;
    [SerializeField] public bool isLocked = false;

    public static Action OnItemBeginDrag;
    public static Action OnItemEndDrag;

    private Image image;
    [HideInInspector]
    public Transform parentAfterDrag;
    [HideInInspector]
    public Transform parentBeforeDrag;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        parentAfterDrag = transform.parent;
        parentBeforeDrag = transform.parent;

        transform.SetParent(transform.parent.parent);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        OnItemBeginDrag?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        RectTransform rt = (RectTransform)transform;
        transform.localPosition = Input.mousePosition + new Vector3(-rt.rect.width / 4, rt.rect.height / 4, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        
        OnItemEndDrag?.Invoke();
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;

        if (isDestroyable && transform.parent != parentBeforeDrag && !isLocked)
        {
            Destroy(gameObject);
        }

    }
}
