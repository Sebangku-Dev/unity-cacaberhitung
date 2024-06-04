using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private RectTransform rectTransform;
    public Canvas canvas;
    private RectTransform rectCanvas;
    private CanvasGroup canvasGroup;

    public Image image;
    [HideInInspector] public Transform parentAfterDrag;

    public GameObject item;
    public GameObject itemTarget;
    public int minDistance;

    public bool isResetPosition;
    public bool isHasTarget;
    public bool isChangeRotate;
    public bool isChangeScale;

    Vector3 initialPosition;

    void Start()
    {        
        if (isHasTarget || isResetPosition)
        {
            initialPosition = transform.position;
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (canvas != null) rectCanvas = canvas.GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canvas != null) rectCanvas = canvas.GetComponent<RectTransform>();
        if (rectCanvas != null) rectTransform.anchoredPosition = ClampToCanvas(rectTransform.anchoredPosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isHasTarget)
        {
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.parent.parent);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float distance = Vector3.Distance(item.transform.localPosition, itemTarget.transform.localPosition);

        if (isHasTarget)
        {

            if (distance < minDistance)
            {
                item.transform.localPosition = itemTarget.transform.localPosition;
                transform.SetParent(parentAfterDrag);

                if (isChangeRotate)
                {
                    item.transform.rotation = Quaternion.Euler(0f, 0f, itemTarget.transform.rotation.eulerAngles.z);
                }

                if (isChangeScale)
                {
                    RectTransform rectItem = item.GetComponent<RectTransform>();
                    RectTransform rectTarget = itemTarget.GetComponent<RectTransform>();

                    rectItem.localScale = rectTarget.localScale;
                }
            }

            else
            {
                transform.position = initialPosition;
            }
        }
        else if (isResetPosition)
        {
            transform.position = initialPosition;
        }

        image.raycastTarget = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }

    private Vector2 ClampToCanvas(Vector2 position)
    {
        // Hitung batas-batas di mana objek bisa berada
        float minX = (rectCanvas.rect.width * -0.5f) + (rectTransform.rect.width * rectTransform.pivot.x);
        float maxX = (rectCanvas.rect.width * 0.5f) - (rectTransform.rect.width * (1 - rectTransform.pivot.x));

        // Perbaikan: Menggunakan pivot.y untuk menghitung minY dan maxY
        float minY = (rectCanvas.rect.height * -0.5f) + (rectTransform.rect.height * rectTransform.pivot.y);
        float maxY = (rectCanvas.rect.height * 0.5f) - (rectTransform.rect.height * (1 - rectTransform.pivot.y));

        // Batasi posisi objek agar tidak keluar dari Canvas
        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(position.x, minX, maxX),
            Mathf.Clamp(position.y, minY, maxY)
        );

        return clampedPosition;
    }
}
