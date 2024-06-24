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

    [SerializeField] GameObject[] panels;
    [SerializeField] private Loading loaderCanvas;
    private void ActivateLoaderCanvas() => Instantiate(loaderCanvas.gameObject);
    private void DeactivateLoaderCanvas() => loaderCanvas.gameObject.SetActive(false);
    private string lastScenesString = "";
    private const int HOME_SCENE_INDEX = 1;

    public async void LoadScene(string targetScene, bool isSaveToStack = true)
    {
        // Get build scene index
        lastSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (PlayerPrefs.GetString("sceneStack") == null)
            return;


        // Condition to push build indesx to lastSceneString
        if (isSaveToStack)
        {
            if (lastScenesString.Length > 0)
            {
                lastScenesString += $";{lastSceneIndex}";
            }
            else
            {
                lastScenesString += $"{lastSceneIndex}";
            }
        }


        var scene = SceneManager.LoadSceneAsync(targetScene);
        scene.allowSceneActivation = false; // prevent screen for immediate load

        ActivateLoaderCanvas();

        await Task.Delay(500); // same as WaitForSecond but in async await
        scene.allowSceneActivation = true;
    }

    public async void Back()
    {
        int latestSceneIndex = 0;

        // if (PlayerPrefs.GetString("sceneStack") != null)
        //     lastScenesString = PlayerPrefs.GetString("sceneStack");


        if (lastScenesString.Length > 1)
        {
            // Get last scene string to be loaded
            latestSceneIndex = Int16.Parse(lastScenesString.Split(";").Last());

            // Release last scene string
            lastScenesString = lastScenesString.Substring(0, lastScenesString.Length - 2);
        }
        else if (lastScenesString.Length == 1)
        {
            latestSceneIndex = Int16.Parse(lastScenesString);
            lastScenesString = lastScenesString.Substring(0, lastScenesString.Length - 1);
        }
        else
        {
            latestSceneIndex = HOME_SCENE_INDEX;
        }


        // PlayerPrefs.SetString("sceneStack", lastScenesString);

        var scene = SceneManager.LoadSceneAsync(latestSceneIndex);
        scene.allowSceneActivation = false;

        ActivateLoaderCanvas();

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

        IAnimate animation = Notification.GetComponent<IAnimate>();
        animation.Load();
    }
}
