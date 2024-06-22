

using TMPro;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{

    protected override void Awake()
    {
        base.Awake();
    }

    public void AddScore(int totalScore)
    {
        DataSystem.Instance.User.currentScore += totalScore;
    }

    private int GetCurrentScore()
    {
        return DataSystem.Instance.User.currentScore;
    }

}