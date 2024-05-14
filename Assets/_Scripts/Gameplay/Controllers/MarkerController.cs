using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MarkerController : MonoBehaviour
{
    public static bool isOnMarkerArea;
    [SerializeField] public Level levelToPlay;
    [SerializeField] public GameObject BarrierToUnlock;
    // [SerializeField] NavigationSystem nav;

    [SerializeField] TextMeshProUGUI TextTitle, TextDetail;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TextTitle.text = levelToPlay.title;
            TextDetail.text = levelToPlay.description;
            
            ToggleMissionPanel();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ToggleMissionPanel();
        }
    }

    void ToggleMissionPanel()
    {
        NavigationSystem.Instance.TogglePanel(1);
        MarkerController.isOnMarkerArea = !MarkerController.isOnMarkerArea;
    }

    public void PlayLevel()
    {
        NavigationSystem.Instance.LoadScene(levelToPlay.levelSceneName);
    }
}