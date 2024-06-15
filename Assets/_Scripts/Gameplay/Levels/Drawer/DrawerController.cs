using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawerController : Singleton<DrawerController>, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private Line line;
    [SerializeField] private Camera cam;

    private Line currentLine;

    public const float RESOLUTION = 0.1f;
    public static Action OnPointerMoveEvent;
    public static Action OnPointerUpEvent;


    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        if (Input.GetMouseButtonDown(0)) currentLine = Instantiate(line, mousePos, Quaternion.identity, transform);

        if (Input.GetMouseButton(0)) currentLine.SetPosition(mousePos);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
            OnPointerMoveEvent.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset after drawing
        Destroy(currentLine.gameObject);
        OnPointerUpEvent?.Invoke();
    }
}
