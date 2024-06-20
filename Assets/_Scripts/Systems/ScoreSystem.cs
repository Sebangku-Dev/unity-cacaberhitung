

using TMPro;
using UnityEngine;

public class ScoreSystem : SingletonPersistent<ScoreSystem>
{
    [SerializeField] public int Score;

    protected override void Awake()
    {
        base.Awake();
    }

    public void AddScore(int totalScore)
    {
        ScoreSystem.Instance.Score += totalScore;
    }

    private void LoadFromUser()
    {
        ScoreSystem.Instance.Score = UserManager.Instance.GetCurrentUser().currentScore;
    }

}