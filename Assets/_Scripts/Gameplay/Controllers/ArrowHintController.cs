using UnityEngine;

public class ArrowHintController : MonoBehaviour
{

    private Level targetLevel;

    private void Start() => targetLevel = LevelManager.Instance.GetReadyToPlayLevel();
    private void Update() => RotateArrowTowardsMission();

    void RotateArrowTowardsMission()
    {
        if (targetLevel)
        {
            Vector3 directionToMarker = targetLevel.marker.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToMarker);

            // Constraint rotate only on y axis
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 100.0f);
        }
    }

    public void OnToggleHint() => gameObject.SetActive(!gameObject.activeSelf);
}
