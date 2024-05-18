using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    [SerializeField] GameObject Notification;
    [SerializeField] TextMeshProUGUI TextNotifTitle, TextNotifMessage;

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
                    PopupAnimation animation = panel.GetComponent<PopupAnimation>();
                    if (animation)
                    {
                        if (!panel.activeSelf) animation.Load();
                        else _ = animation.Close();
                    }
                    else panel.SetActive(!panel.activeSelf);
                    if (index > 1)
                    {
                        panels[0].SetActive(panel.activeSelf == false);

                        if (panel.activeSelf == false && MarkerController.isOnMarkerArea)
                        {
                            PopupAnimation anim = panels[1].GetComponent<PopupAnimation>();
                            if (anim) anim.Load();
                        }
                    }
                    else if (index == 1) panels[0].SetActive(panel.activeSelf == false);
                }
                else panel.SetActive(false);

                i++;
            }
        }
    }

    public void ToggleNotification(string content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            string[] subs = content.Split('|');

            if (subs[0] != null) TextNotifTitle.text = subs[0];
            if (subs[1] != null) TextNotifMessage.text = subs[1];
        }

        PopupAnimation animation = Notification.GetComponent<PopupAnimation>();
        animation.Load();
    }
}
