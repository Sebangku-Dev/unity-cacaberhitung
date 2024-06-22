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
        public Transform title;
        public string text;
        public TMP_InputField input;
        public Button nextButton;
    }

    [SerializeField] List<Field> fields;
    [SerializeField] Button asQuestButton, toHome;
    private string inputName, inputAge;

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
        if (step > 0 && step < fields.Count)
        {
            fields[step].nextButton.interactable = !string.IsNullOrEmpty(fields[step]?.input?.text);
            SaveState();
        }

    }

    private void ShowPanel(int index)
    {
        fields[index].panel.gameObject.SetActive(true);

        if (fields[index].title.GetComponent<TextMeshProUGUI>() != null) fields[index].title.GetComponent<TextMeshProUGUI>().text = fields[index].text;
        fields[index].title.gameObject.SetActive(true);
    }

    private void HidePanel(int index)
    {
        fields[index].panel.gameObject.SetActive(false);
        fields[index].title.gameObject.SetActive(false);
    }

    public void ShowPanelByTransform(Transform panel)
    {
        panel.gameObject.SetActive(true);
    }

    public void HidePanelByTransform(Transform panel)
    {
        panel.gameObject.SetActive(true);
    }

    public void SaveState()
    {
        if (fields[step]?.input?.name == "Input Name") inputName = fields[step].input.text;
        if (fields[step]?.input?.name == "Input Age") inputAge = fields[step].input.text;
    }

    public void IncrementStep()
    {
        HidePanel(step);
        step++;
        ShowPanel(step);
    }

    public void DecrementStep()
    {
        HidePanel(step);
        step--;
        ShowPanel(step);
    }

    public void OnRegisterSubmit()
    {
        if (inputName != null && inputAge != null)
            UserManager.Instance.Register(inputName, inputAge);
        else
            UserManager.Instance.Register("New User", "3");

    }
}
