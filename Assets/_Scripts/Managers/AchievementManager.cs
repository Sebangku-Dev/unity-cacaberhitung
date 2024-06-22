using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AchievementManager : Singleton<AchievementManager>
{

    private void Start()
    {
        EvaluatePossibleAchievement();
    }

    public List<Achievement> GetAllAchievements()
    {
        return DataSystem.Instance.Achievements;
    }

    public List<Achievement> GetUnlockedAchievements()
    {
        return DataSystem.Instance.Achievements.Where(achievement => achievement.isUnlocked).ToList();
    }

    private void UnlockAchievement(Achievement achievement)
    {
        achievement.isUnlocked = true;
    }

    #region Achievement Evaluator

    public async void EvaluatePossibleAchievement()
    {
        await Task.WhenAll(new Task[] {
            EvaluateAchievement1(),
            EvaluateAchievement2(),
            EvaluateAchievement3(),
            EvaluateAchievement4(),
            EvaluateAchievement5(),
            EvaluateAchievement6(),
            EvaluateAchievement7(),
            EvaluateAchievement8(),
            EvaluateAchievement9(),
        });
    }

    private async Task EvaluateAchievement1()
    {
        if (DataSystem.Instance.Levels.Where(level => level.location.region == "Firstaria Island").All(level => level.isSolved))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 1));
        }

        await Task.Yield();
    }

    private async Task EvaluateAchievement2()
    {
        if (DataSystem.Instance.Levels.Where(level => level.location.region == "Secondela Island").All(level => level.isSolved))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 2));
        }

        await Task.Yield();
    }
    private async Task EvaluateAchievement3()
    {
        if (DataSystem.Instance.Levels.Where(level => level.location.region == "Thirdema Island").All(level => level.isSolved))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 3));
        }

        await Task.Yield();
    }
    private async Task EvaluateAchievement4()
    {
        if (DataSystem.Instance.Levels.All(level => level.isSolved))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 4));
        }

        await Task.Yield();
    }
    private async Task EvaluateAchievement5()
    {
        if (DataSystem.Instance.Levels.Find(level => level.id == 11).isSolved)
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 5));
        }

        await Task.Yield();
    }
    private async Task EvaluateAchievement6()
    {
        if (DataSystem.Instance.Levels.All(level => level.isRightInTime))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 6));
        }

        await Task.Yield();
    }
    private async Task EvaluateAchievement7()
    {
        if (DataSystem.Instance.Levels.All(level => level.isNoMistake))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 7));
        }

        await Task.Yield();
    }
    private async Task EvaluateAchievement8()
    {
        if (DataSystem.Instance.Levels.All(level => level.isNoMistake && level.isSolved && level.isRightInTime))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 8));
        }

        await Task.Yield();
    }
    private async Task EvaluateAchievement9()
    {
        if (DataSystem.Instance.Levels.Sum(level => level.playCount) > 50)
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 9));
        }

        await Task.Yield();
    }

    #endregion

}
