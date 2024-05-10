using UnityEngine;

public class MarkerController : MonoBehaviour
{
    [SerializeField] Level levelToPlay;
    // [SerializeField] NavigationSystem nav;

    public void PlayLevel()
    {
        NavigationSystem.Instance.LoadScene(levelToPlay.levelSceneName);
    }
}