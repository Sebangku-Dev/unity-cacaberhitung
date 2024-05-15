using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used for navigation system accross the scenes
/// </summary>
public class NavigationSystem : SingletonPersistent<NavigationSystem>
{
    private static int lastScene;

    [SerializeField] GameObject[] panels;
    [SerializeField] private Loading loaderCanvas;
    private void ActivateLoaderCanvas() => loaderCanvas.gameObject.SetActive(true);
    private void DeactivateLoaderCanvas() => loaderCanvas.gameObject.SetActive(false);

    public async void LoadScene(string targetScene)
    {
        lastScene = SceneManager.GetActiveScene().buildIndex;

        var scene = SceneManager.LoadSceneAsync(targetScene);
        scene.allowSceneActivation = false; // prevent screen for immediate load
        ActivateLoaderCanvas();

        /*
            you can implement progress method here
        */

        await Task.Delay(1000); // same as WaitForSecond but in async await

        scene.allowSceneActivation = true;

        // Disable this for better UX
        // DeactivateLoaderCanvas();
    }

    public void Back()
    {
        SceneManager.LoadScene(lastScene);
    }

    public void TogglePanel(int index)
    {
        if (panels != null)
        {
            int i = 0;
            foreach (GameObject panel in panels)
            {
                if (i == index)
                {
                    panel.SetActive(!panel.activeSelf);
                    if (index > 1)
                    {
                        panels[0].SetActive(panel.activeSelf == false);
                        panels[1].SetActive(panel.activeSelf == false && MarkerController.isOnMarkerArea);
                    }
                    else if (index == 1) panels[0].SetActive(panel.activeSelf == false);
                }
                else panel.SetActive(false);

                i++;
            }
        }
    }
}
