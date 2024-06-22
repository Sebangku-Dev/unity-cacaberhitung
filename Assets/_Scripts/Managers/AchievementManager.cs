using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class AchievementManager : Singleton<AchievementManager>
{

    [SerializeField] private bool isEvaluateOnStart = false;
    [SerializeField] protected AchievementToast achievementToastPrefab;


    private bool isAnyUnlocked = false;


    private void Start()
    {
        if (isEvaluateOnStart)
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
        if (achievement.isUnlocked) return;

        achievement.isUnlocked = true;
        isAnyUnlocked = true;
    }

    #region Achievement Evaluator

    protected void Toast()
    {
        var instantiatedToast = Instantiate(achievementToastPrefab, FindObjectsOfType<Canvas>().ToList().Find(canvas => canvas.name == "UI Canvas").transform);
        instantiatedToast.GetComponent<IAnimate>().Load();
    }

    public void EvaluatePossibleAchievement()
    {
        isAnyUnlocked = false;

        EvaluateAchievement1();
        EvaluateAchievement2();
        EvaluateAchievement3();
        EvaluateAchievement4();
        EvaluateAchievement5();
        EvaluateAchievement6();
        EvaluateAchievement7();
        EvaluateAchievement8();
        EvaluateAchievement9();

        if (isAnyUnlocked) Toast();
    }

    private void EvaluateAchievement1()
    {
        if (DataSystem.Instance.Levels.Where(level => level.location.region == "Firstaria Island").All(level => level.isSolved))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 1));
        }
    }

    private void EvaluateAchievement2()
    {
        if (DataSystem.Instance.Levels.Where(level => level.location.region == "Secondela Island").All(level => level.isSolved))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 2));
        }
    }
    private void EvaluateAchievement3()
    {
        if (DataSystem.Instance.Levels.Where(level => level.location.region == "Thirdema Island").All(level => level.isSolved) && false)
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 3));
        }
    }
    private void EvaluateAchievement4()
    {
        if (DataSystem.Instance.Levels.All(level => level.isSolved))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 4));
        }
    }
    private void EvaluateAchievement5()
    {
        if (DataSystem.Instance.Levels.Find(level => level.id == 11).isSolved)
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 5));
        }
    }
    private void EvaluateAchievement6()
    {
        if (DataSystem.Instance.Levels.All(level => level.isRightInTime))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 6));
        }
    }
    private void EvaluateAchievement7()
    {
        if (DataSystem.Instance.Levels.All(level => level.isNoMistake))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 7));
        }
    }
    private void EvaluateAchievement8()
    {
        if (DataSystem.Instance.Levels.All(level => level.isNoMistake && level.isSolved && level.isRightInTime))
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 8));
        }
    }
    private void EvaluateAchievement9()
    {
        if (DataSystem.Instance.Levels.Sum(level => level.playCount) > 50)
        {
            UnlockAchievement(DataSystem.Instance.Achievements.Find(achievement => achievement.id == 9));
        }
    }

    #endregion

}
