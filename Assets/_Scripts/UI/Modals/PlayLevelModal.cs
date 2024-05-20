using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayLevelModal : Modal
{
    [SerializeField] private TextMeshProUGUI titleText, descriptionText;
    [SerializeField] private Button buttonStart;

    public void ActivatePlayLevelModal(Level relatedLevel)
    {
        titleText.text = relatedLevel.name;
        descriptionText.text = relatedLevel.description;
        buttonStart.onClick.AddListener(() => NavigationSystem.Instance.LoadScene(relatedLevel.levelSceneName));

        base.ActivateModal();
    }

    public void DeactivatePlayLevelModal()
    {
        base.DeactivateModal();
    }


}
