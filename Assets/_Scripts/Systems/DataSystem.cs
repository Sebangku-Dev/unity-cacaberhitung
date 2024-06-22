using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataSystem : SingletonPersistent<DataSystem>
{
    public User User { get; set; }
    public List<Level> Levels { get; private set; }
    public List<Achievement> Achievements { get; private set; }
    public List<Knowledge> Knowledge { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/game.save"))
        {
            Load();
        }

        if (User == null) return;

        // Load from Saved User Data
        MapSavedToSO(User.savedLevels);
        MapSavedToSO(User.savedAchievement);
    }

    private void AssembleResources()
    {
        Levels = Resources.LoadAll<Level>("Levels").ToList();
        Knowledge = Resources.LoadAll<Knowledge>("Knowledges").ToList();
        Achievements = Resources.LoadAll<Achievement>("Achievements").ToList();
    }

    public void Save(User user)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/game.save");
        binaryFormatter.Serialize(file, user);
        file.Close();

        Load();
    }

    public void Load()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/game.save", FileMode.Open);
        User user = (User)binaryFormatter.Deserialize(file);
        file.Close();

        Debug.Log("Game Save is Exist");
        Debug.Log("User: " + JsonUtility.ToJson(user));

        User = user;
    }

    private void MapSavedToSO(List<SaveLevel> savedLevels)
    {
        // Better performance but savedLevel == Levels in terms of array size and item location
        for (int i = 0; i < savedLevels.Count; i++)
        {
            if (savedLevels[i].id == Levels[i].id)
            {
                Levels[i].isUnlocked = savedLevels[i].isUnlocked;
                Levels[i].isSolved = savedLevels[i].isSolved;
                Levels[i].isRightInTime = savedLevels[i].isRightInTime;
                Levels[i].isNoMistake = savedLevels[i].isNoMistake;
                Levels[i].isToBePlayed = savedLevels[i].isToBePlayed;
                Levels[i].playCount = savedLevels[i].playCount;
            }
        }
    }

    private void MapSavedToSO(List<SaveAchievement> savedAchievements)
    {
        // Better performance but savedLevel == Levels in terms of array size and item location
        for (int i = 0; i < savedAchievements.Count; i++)
        {
            if (savedAchievements[i].id == Achievements[i].id)
            {
                Achievements[i].isUnlocked = savedAchievements[i].isUnlocked;
            }
        }
    }

}
