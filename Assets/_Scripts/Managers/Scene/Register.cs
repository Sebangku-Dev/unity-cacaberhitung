using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    [SerializeField] GameObject[] panels;
    [SerializeField] Button asQuestButton;
    [SerializeField] Button[] nextButton;
    [SerializeField] TMP_InputField inputName, inputAge;
    // Start is called before the first frame update
    void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/game.save")) NavigationSystem.Instance.LoadScene("Home");
    }

    // Update is called once per frame
    void Update()
    {
        nextButton[1].interactable = !string.IsNullOrEmpty(inputName.text);
        nextButton[2].interactable = !string.IsNullOrEmpty(inputAge.text);
    }

    public void ToggleCreateState(int index)
    {
        int i = 0;
        foreach (GameObject panel in panels)
        {
            panel.SetActive(i == index);
            i++;
        }

        Debug.Log(inputName.text + " - " + inputAge.text);
    }

    public void SubmitRegister()
    {
        UserManager.Instance.NewUser = new()
        {
            id = inputName.text + DateTime.Now.ToString("yyyy-MM-dd"),
            name = inputName.text,
            age = Int32.Parse(inputAge.text)
        };

        UserManager.Instance.Save();

        // Debug.Log( UserManager.Instance.User.name);
    }

    public void OnClickStart()
    {
        NavigationSystem.Instance.LoadScene("Home");
    }
}
