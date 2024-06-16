using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPoin : MonoBehaviour
{
    [Header("Point")]
    [SerializeField] TextMeshProUGUI pointText;
    [Header("Achievement")]
    [SerializeField] Button achievementButton;
    [Header("Knowledge")]
    [SerializeField] Button knowledgeButton;

    void Start()
    {
        pointText.text = ScoreSystem.Instance.Score.ToString();
        achievementButton.onClick.AddListener(() => NavigationSystem.Instance.LoadScene("MenuAchievements"));
        knowledgeButton.onClick.AddListener(() => NavigationSystem.Instance.LoadScene("MenuKnowledge"));
    }
}
