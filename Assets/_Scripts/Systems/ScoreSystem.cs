using Unity.VisualScripting;

public class ScoreSystem : SingletonPersistent<ScoreSystem>
{
    private int totalScore;

    public void AddScore(int totalScore)
    {
        this.totalScore = totalScore;
        UserManager.Instance.User.currentScore = this.totalScore;
    }
}