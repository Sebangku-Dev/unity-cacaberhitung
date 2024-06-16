using UnityEngine;
using UnityEngine.UI;

public class LevelEndedModal : Modal
{
    [SerializeField] Image starSolved;
    [SerializeField] Image starTime;
    [SerializeField] Image starPerfect;

    // Overriding parent modal but with modified parameter from parent
    public void ActivateEndedModal(bool isSolved, bool isRightInTime, bool isNoMistake)
    {        
        // Star result at the end
        starSolved.gameObject.SetActive(isSolved);
        starTime.gameObject.SetActive(isRightInTime);
        starPerfect.gameObject.SetActive(isNoMistake);

        base.ActivateModal();
    }
}