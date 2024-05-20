

using UnityEngine;

public class ScoreSystem : SingletonPersistent<ScoreSystem>
{
    public int totalScore { get; private set; }

    private void Start()
    {
        this.totalScore = UserManager.Instance.User.currentScore;
    }

    public void AddScore(int totalScore)
    {
        this.totalScore = totalScore;
        UserManager.Instance.User.currentScore = this.totalScore;
    }
}