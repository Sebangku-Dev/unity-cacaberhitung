using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavigationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private Button backButton, pointButton, menuButton;

    private void Start()
    {
        if (UserManager.Instance.User != null) scoreText.text = UserManager.Instance.User.currentScore.ToString();

        backButton.onClick.AddListener(NavigationSystem.Instance.Back);

        pointButton.onClick.AddListener(() => NavigationSystem.Instance.LoadScene("MenuPoin"));

        menuButton.onClick.AddListener(() => NavigationSystem.Instance.LoadScene("MenuLevel"));
    }
}
