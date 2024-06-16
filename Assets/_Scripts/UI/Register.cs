using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{

    [Serializable]
    public class Field
    {
        public Transform panel;
        public TMP_InputField input;
        public Button nextButton;
    }

    [SerializeField] List<Field> fields;
    [SerializeField] Button asQuestButton, toHome;
    [SerializeField] TMP_InputField inputName, inputAge;

    private int step = 0;

    void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/game.save")) NavigationSystem.Instance.LoadScene("Home");

        toHome.onClick.AddListener(() => NavigationSystem.Instance.LoadScene("Home"));

        ShowPanel(step);
    }

    void Update()
    {
        // If input is empty then disable the correlated button
        if (step > 0)
        {
            fields[step].nextButton.interactable = !string.IsNullOrEmpty(fields[step]?.input?.text);
        }

    }

    private void ShowPanel(int index) => fields[index].panel.gameObject.SetActive(true);
    private void HidePanel(int index) => fields[index].panel.gameObject.SetActive(false);

    public void IncrementStep()
    {
        HidePanel(step);
        step++;
        ShowPanel(step);
    }

    public void SubmitRegister()
    {
        UserManager.Instance.NewUser = new User()
        {
            id = inputName.text + DateTime.Now.ToString("yyyy-MM-dd"),
            name = inputName.text,
            age = Int32.Parse(inputAge.text)
        };

        UserManager.Instance.Save();
    }
}
