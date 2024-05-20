using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using System;

/// <summary>
/// Used for navigation system accross the scenes
/// </summary>
public class NavigationSystem : SingletonPersistent<NavigationSystem>
{
    private int lastSceneIndex;

    /// <summary>
    /// ;1;2;3;4;5;6 -> 1 is oldest, 6 is latest. Index starts from 1
    /// </summary>
    private string lastScenesString = "";

    [SerializeField] GameObject[] panels;
    [SerializeField] private Loading loaderCanvas;
    private void ActivateLoaderCanvas() => Instantiate(loaderCanvas.gameObject);
    private void DeactivateLoaderCanvas() => loaderCanvas.gameObject.SetActive(false);

    public async void LoadScene(string targetScene)
    {
        // Get build scene index
        lastSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Condition to push build indesx to lastSceneString
        if (lastScenesString.Length > 0) { lastScenesString += $";{lastSceneIndex}"; }
        else lastScenesString += $"{lastSceneIndex}";

        Debug.Log(lastScenesString);

        var scene = SceneManager.LoadSceneAsync(targetScene);
        scene.allowSceneActivation = false; // prevent screen for immediate load
        ActivateLoaderCanvas();

        /*
            you can implement progress method here
        */

        await Task.Delay(500); // same as WaitForSecond but in async await
        scene.allowSceneActivation = true;
    }

    public async void Back()
    {
        int latestSceneIndex = 0;

        if (lastScenesString.Length > 1)
        {
            latestSceneIndex = Int16.Parse(lastScenesString.Split(";").Last());
            lastScenesString = lastScenesString.Substring(0, lastScenesString.Length - 2);
        }
        else
            latestSceneIndex = Int16.Parse(lastScenesString);

        var scene = SceneManager.LoadSceneAsync(latestSceneIndex);
        scene.allowSceneActivation = false;
        ActivateLoaderCanvas();
        /*
            you can implement progress method here
        */
        await Task.Delay(500); // same as WaitForSecond but in async await
        scene.allowSceneActivation = true;
    }

    [SerializeField] GameObject Notification;
    [SerializeField] TextMeshProUGUI TextNotifTitle, TextNotifMessage;

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
                        else animation.Close();
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
