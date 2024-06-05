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

    [HideInInspector]
    public Transform parentAfterDrag;
    [HideInInspector]
    public Transform parentBeforeDrag;

    private Image image;
    private Canvas canvas;
    private RectTransform canvasRectTransform, rectTransform;

    private void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        canvas = GameObject.Find("UI Canvas").GetComponent<Canvas>(); // Not recommended to get a gameobject just by name
        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        if (isLocked) return;

        parentAfterDrag = transform.parent;
        parentBeforeDrag = transform.parent;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        if (GetComponent<CanvasGroup>() != null)
            GetComponent<CanvasGroup>().blocksRaycasts = false;

        OnItemBeginDrag?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        rectTransform.anchoredPosition = ClampToCanvas(rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        OnItemEndDrag?.Invoke();

        transform.SetParent(parentAfterDrag);

        image.raycastTarget = true;

        if (GetComponent<CanvasGroup>() != null)
            GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (isDestroyable && transform.parent != parentBeforeDrag && !isLocked)
        {
            Destroy(gameObject);
        }

    }

    // Bound draggable item to canvas -> must be left top rect
    private Vector2 ClampToCanvas(Vector2 position)
    {
        // Hitung batas-batas di mana objek bisa berada
        float minX = (canvasRectTransform.rect.width * -rectTransform.pivot.x) + (rectTransform.rect.width * rectTransform.pivot.x);
        float maxX = (canvasRectTransform.rect.width * (1 - rectTransform.pivot.x)) - (rectTransform.rect.width * (1 - rectTransform.pivot.x));

        // Perbaikan: Menggunakan pivot.y untuk menghitung minY dan maxY
        float minY = (canvasRectTransform.rect.height * -rectTransform.pivot.y) + (rectTransform.rect.height * rectTransform.pivot.y);
        float maxY = (canvasRectTransform.rect.height * (1 - rectTransform.pivot.y)) - (rectTransform.rect.height * (1 - rectTransform.pivot.y));

        // Batasi posisi objek agar tidak keluar dari Canvas
        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(position.x, minX < maxX ? minX : maxX, minX > maxX ? minX : maxX),
            Mathf.Clamp(position.y, minY < maxX ? minY : maxY, minY > maxX ? minY : maxY)
        );

        return clampedPosition;
    }
}
