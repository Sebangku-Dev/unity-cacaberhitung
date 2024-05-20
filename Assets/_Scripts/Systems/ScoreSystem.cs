

using UnityEngine;

public class ScoreSystem : SingletonPersistent<ScoreSystem>
{
    public int Score { get; private set; }

    private void Start()
    {
        this.Score = UserManager.Instance.User.currentScore;
    }

    public void AddScore(int totalScore)
    {
        this.Score = totalScore;
        UserManager.Instance.User.currentScore = this.Score;
    }
}