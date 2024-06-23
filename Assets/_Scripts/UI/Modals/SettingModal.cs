using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingModal : Modal
{
    [SerializeField] private Button toggleMusic;

    private bool isChecked = true;
    private Transform checkmark;


    private void Start()
    {
        checkmark = toggleMusic.transform.GetChild(0);

        // Set based on current mute state
        isChecked = !AudioSystem.Instance.IsMute;
        checkmark.gameObject.SetActive(isChecked);
    }

    public void ActivateSettingModal()
    {
        toggleMusic.onClick.AddListener(() => Toggle(checkmark));

        base.ActivateModal();
    }

    public void DeactivateSettingModal()
    {
        base.DeactivateModal();
    }

    private void Toggle(Transform checkmark)
    {
        checkmark.gameObject.SetActive(!isChecked);
        isChecked = checkmark.gameObject.activeSelf;

        AudioSystem.Instance.SetMute(!isChecked);
    }
}
