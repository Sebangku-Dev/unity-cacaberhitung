using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private string targetScene;

    private void LoadScene()
    {
        SceneManager.LoadSceneAsync("");
    }
}
