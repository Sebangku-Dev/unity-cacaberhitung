using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavigationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private Button backButton, pointButton, menuButton;


    private void Start()
    {
        backButton.onClick.AddListener(NavigationSystem.Instance.Back);
        pointButton.onClick.AddListener(() => NavigationSystem.Instance.LoadScene("MenuPoin"));
        menuButton.onClick.AddListener(() => NavigationSystem.Instance.LoadScene("MenuLevel"));
    }

    private void Update()
    {
        WatchScoreChange();
    }

    private void WatchScoreChange()
    {
        scoreText.text = DataSystem.Instance?.User?.currentScore + "" ?? "0";
    }

}
