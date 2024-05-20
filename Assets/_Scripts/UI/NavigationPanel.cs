using TMPro;
using UnityEngine;

public class NavigationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Update()
    {
        scoreText.text = ScoreSystem.Instance.totalScore.ToString();
    }
}
