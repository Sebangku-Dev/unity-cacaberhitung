using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;
using System.Linq;
public class UserManager : Singleton<UserManager>
{

    [Header("Login as Guest")]
    [SerializeField] public User NewUser;

    public void Register(string name, string age)
    {
        UserManager.Instance.NewUser = new User()
        {
            id = name + DateTime.Now.ToString("yyyy-MM-dd"),
            name = name,
            age = Int32.Parse(age),
            currentScore = 0,
        };

        foreach (Level level in DataSystem.Instance.Levels)
        {
            SaveLevel saved = new()
            {
                id = level.id,
                isUnlocked = level.id == 1,
                isToBePlayed = level.id == 1,
            };

            UserManager.Instance.NewUser.savedLevels.Add(saved);
        }

        foreach (Knowledge knowledge in DataSystem.Instance.Knowledge)
        {
            SaveKnowledge saved = new()
            {
                id = knowledge.id,
                isCollected = false
            };

            UserManager.Instance.NewUser.savedKnowledge.Add(saved);
        }

        foreach (Achievement a in DataSystem.Instance.Achievements)
        {
            SaveAchievement saved = new()
            {
                id = a.id,
                isUnlocked = false
            };

            UserManager.Instance.NewUser.savedAchievement.Add(saved);
        }

        // Save New User to game.save
        DataSystem.Instance.Save(NewUser);

        Debug.Log(JsonUtility.ToJson(UserManager.Instance.GetCurrentUser()));
    }

    public User GetCurrentUser() => DataSystem.Instance.User;

    /// <summary>
    /// Add to Data System
    /// </summary>
    /// <param name="level">Which level</param>
    public void AddSavedLevel(Level level)
    {
        int levelListIndex = DataSystem.Instance.User.savedLevels.FindIndex(l => l.id == level.id);

        DataSystem.Instance.User.savedLevels[levelListIndex] = new SaveLevel()
        {
            id = level.id,
            isUnlocked = level.isUnlocked,
            isSolved = level.isSolved,
            isRightInTime = level.isRightInTime,
            isNoMistake = level.isNoMistake,
            isToBePlayed = level.isToBePlayed,
            playCount = level.playCount
        };
    }

    /// <summary>
    /// Add to Data System
    /// </summary>
    /// <param name="level">Which achievement</param>
    public void AddSavedAchievement(Achievement achievement)
    {
        int achievementLevelIndex = DataSystem.Instance.User.savedAchievement.FindIndex(a => a.id == achievement.id);

        DataSystem.Instance.User.savedAchievement[achievementLevelIndex] = new SaveAchievement()
        {
            id = achievement.id,
            isUnlocked = achievement.isUnlocked
        };
    }

    

}
