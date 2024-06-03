using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{


    [SerializeField] private bool isDestroyable = false;

    private Image image;
    [HideInInspector]
    public Transform parentAfterDrag;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.parent.parent);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform rt = (RectTransform)transform;
        transform.localPosition = Input.mousePosition + new Vector3(-rt.rect.width / 4, rt.rect.height / 4, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;

            if (isDestroyable && transform.parent.name == "Plate")
            {
                Destroy(gameObject);
            }
    }
}
