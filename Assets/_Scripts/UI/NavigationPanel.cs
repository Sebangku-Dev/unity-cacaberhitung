using TMPro;
using UnityEngine;

public class NavigationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        scoreText.text = UserManager.Instance.User.currentScore.ToString();
    }
}
