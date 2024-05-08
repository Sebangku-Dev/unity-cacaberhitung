using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used for navigation system accross the scenes
/// </summary>
public class NavigationSystem : SingletonPersistent<NavigationSystem>
{
    [SerializeField] private GameObject loaderCanvas;

    public async void LoadScene(string targetScene)
    {
        var scene = SceneManager.LoadSceneAsync(targetScene);
        scene.allowSceneActivation = false; // prevent screen for immediate load
        loaderCanvas.SetActive(true);

        /*
            you can implement progress method here
        */
        await Task.Delay(500); // same as WaitForSecond but in async await

        loaderCanvas.SetActive(false);
        scene.allowSceneActivation = true;
    }
}
