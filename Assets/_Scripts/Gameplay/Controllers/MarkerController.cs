using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MarkerController : MonoBehaviour
{
    [SerializeField] public Level relatedLevel;
    [SerializeField] public PlayLevelModal playLevelModal;
    [SerializeField] public GameObject BarrierToUnlock;

    public static bool isOnMarkerArea;

    private void Start()
    {
        gameObject.SetActive(relatedLevel.isUnlocked);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // ToggleMissionPanel();
            playLevelModal.ActivatePlayLevelModal(relatedLevel);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // ToggleMissionPanel();
            playLevelModal.DeactivatePlayLevelModal();
        }
    }

    void ToggleMissionPanel()
    {
        NavigationSystem.Instance.TogglePanel(1);
        MarkerController.isOnMarkerArea = !MarkerController.isOnMarkerArea;
    }
}