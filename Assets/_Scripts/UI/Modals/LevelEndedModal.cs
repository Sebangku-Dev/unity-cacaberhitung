using UnityEngine;
using UnityEngine.UI;

public class LevelEndedModal : Modal
{
    [SerializeField] Image starSolved;
    [SerializeField] Image starTime;
    [SerializeField] Image starPerfect;
    [SerializeField] Button buttonNext;

    // Overriding parent modal but with modified parameter from parent
    public void ActivateEndedModal(bool isSolved, bool isRightInTime, bool isNoMistake)
    {
        // Star result at the end
        starSolved.gameObject.SetActive(isSolved);
        starTime.gameObject.SetActive(isRightInTime);
        starPerfect.gameObject.SetActive(isNoMistake);

        buttonNext.onClick.AddListener(() => NavigationSystem.Instance.Back());

        base.ActivateModal();
    }
}