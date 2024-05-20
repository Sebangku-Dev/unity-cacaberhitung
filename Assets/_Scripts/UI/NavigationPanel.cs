using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavigationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private Button backButton, poinButton, menuButton;

    private void Start()
    {
        if (UserManager.Instance.User != null) scoreText.text = UserManager.Instance.User.currentScore.ToString();

        backButton.onClick.AddListener(NavigationSystem.Instance.Back);

        poinButton.onClick.AddListener(() => NavigationSystem.Instance.LoadScene("MenuPoin"));

        menuButton.onClick.AddListener(() => NavigationSystem.Instance.LoadScene("MenuLevel"));
    }
}
