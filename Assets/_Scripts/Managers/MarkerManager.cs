using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarkerManager : MonoBehaviour
{
    [SerializeField] GameObject[] Markers;
    int indexLevel;
    Level currentLevel;

    [SerializeField] TextMeshProUGUI MissionTitle, MissionDetail;

    [SerializeField] Transform player;
    [SerializeField] GameObject HintCanvas;
    public float distanceFromPlayer = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        // if (UserManager.Instance.User != null) indexLevel = UserManager.Instance.User.currentLevel;

        int index = 0;
        foreach (GameObject Marker in Markers)
        {
            Marker.SetActive(index == indexLevel);

            if (Marker.TryGetComponent<MarkerController>(out var markerController))
            {
                if (index == indexLevel) currentLevel = markerController.relatedLevel;
                if (index < indexLevel && markerController.BarrierToUnlock != null) markerController.BarrierToUnlock.SetActive(false);
            }

            if (currentLevel != null)
            {
                MissionTitle.text = currentLevel.title;
                MissionDetail.text = currentLevel.hint;
            }

            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (HintCanvas != null && player != null)
        {
            PositionHintAroundPlayer();
            RotateHintTowardsMission();
        }
    }

    void PositionHintAroundPlayer()
    {
        Vector3 directionToMarker = (Markers[indexLevel].transform.position - player.position).normalized;
        HintCanvas.transform.position = player.position + directionToMarker * distanceFromPlayer;
    }

    void RotateHintTowardsMission()
    {
        Vector3 directionToMarker = Markers[indexLevel].transform.position - HintCanvas.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToMarker);
        HintCanvas.transform.rotation = Quaternion.RotateTowards(HintCanvas.transform.rotation, targetRotation, Time.deltaTime * 100.0f); // Adjust rotation speed as needed
    }

    public void OnToggleHint()
    {
        HintCanvas.SetActive(!HintCanvas.activeSelf);
    }
}
