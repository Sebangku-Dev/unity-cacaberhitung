using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used for navigation system accross the scenes
/// </summary>
public class NavigationSystem : SingletonPersistent<NavigationSystem>
{
    private static int lastScene;

    public async void LoadScene(string targetScene)
    {
        lastScene = SceneManager.GetActiveScene().buildIndex;

        var scene = SceneManager.LoadSceneAsync(targetScene);
        scene.allowSceneActivation = false; // prevent screen for immediate load
        MenuManager.Instance?.ActivateLoaderCanvas();

        /*
            you can implement progress method here
        */

        await Task.Delay(1000); // same as WaitForSecond but in async await

        scene.allowSceneActivation = true;

        // Disable this for better UX
        // MenuManager.Instance?.DeactivateLoaderCanvas();
    }

    public void Back()
    {
        SceneManager.LoadScene(lastScene);
    }
}
