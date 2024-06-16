using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    public void SetPosition(Vector3 pos)
    {
        pos.z = 0f;

        if (!CanAppend(pos)) return;

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
    }

    public Vector3 GetLastPosition()
    {
        return lineRenderer.GetPosition(lineRenderer.positionCount - 1);
    }

    private bool CanAppend(Vector3 pos)
    {
        pos.z = 0f;

        // If the last point match with first point in space coord
        if (lineRenderer.positionCount == 0) return true;

        return Vector3.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), pos) > DrawerController.RESOLUTION;
    }
}
