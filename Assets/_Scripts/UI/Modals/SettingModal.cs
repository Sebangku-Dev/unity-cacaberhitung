using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingModal : Modal
{
    [SerializeField] private Button toggleMusic;

    private Transform checkmark;


    private void Start()
    {
        checkmark = toggleMusic.transform.GetChild(0);

        // Set based on current mute state
        checkmark.gameObject.SetActive(!AudioSystem.Instance.IsMute);
        
        toggleMusic.onClick.AddListener(() => ToggleCheckmark());
    }

    public void ActivateSettingModal()
    {

        base.ActivateModal();
    }

    public void DeactivateSettingModal()
    {
        base.DeactivateModal();
    }

    private void ToggleCheckmark()
    {
        bool activeSelf = checkmark.gameObject.activeSelf;

        checkmark.gameObject.SetActive(!activeSelf);

        AudioSystem.Instance.SetMute(!checkmark.gameObject.activeSelf);
    }
}
