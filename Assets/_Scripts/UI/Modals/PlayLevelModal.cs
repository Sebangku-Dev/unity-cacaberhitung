using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayLevelModal : Modal
{
    [SerializeField] private TextMeshProUGUI titleText, descriptionText;
    [SerializeField] private Button buttonStart;
    [SerializeField] Star solved, time, perfect;

    public void ActivatePlayLevelModal(Level relatedLevel)
    {
        titleText.text = relatedLevel.title;
        descriptionText.text = relatedLevel.hint;

        buttonStart.onClick.AddListener(() => NavigationSystem.Instance.LoadScene(relatedLevel.levelSceneName));

        solved.gameObject.SetActive(relatedLevel.isSolved);
        time.gameObject.SetActive(relatedLevel.isRightInTime);
        perfect.gameObject.SetActive(relatedLevel.isNoMistake);

        base.ActivateModal();
    }

    public void DeactivatePlayLevelModal()
    {
        // Prevent for duplicated level scene load 
        buttonStart.onClick.RemoveAllListeners();

        base.DeactivateModal();
    }
}
