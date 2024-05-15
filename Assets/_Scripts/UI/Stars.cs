using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stars : MonoBehaviour
{
    [SerializeField] Level level;
    [SerializeField] Image starSolved;
    [SerializeField] Image starTime;
    [SerializeField] Image starPerfect;

    private void Start()
    {

    }
    private void Update()
    {
        starSolved.gameObject.SetActive(level.isSolved);
        starTime.gameObject.SetActive(level.isRightInTime);
        starPerfect.gameObject.SetActive(level.isNoMistake);
    }
}
