using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;
public class UserManager : Singleton<UserManager>
{
    [SerializeField] private bool isLoadData;

    [Header("Login as Guest")]
    [SerializeField] public User NewUser;

    private void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/game.save"))
        {
            Load();
        }
    }

    public User CreateUserFile()
    {
        User user = UserManager.Instance.NewUser ?? new User();

        foreach (Level level in DataSystem.Instance.Levels)
        {
            SaveLevel saved = new()
            {
                id = level.id,
            };

            user.listOfSaveLevel.Add(saved);
        }

        foreach (Knowledge knowledge in DataSystem.Instance.Knowledge)
        {
            SaveKnowledge saved = new()
            {
                id = knowledge.id,
                isCollected = false
            };

            user.listOfSaveKnowledge.Add(saved);
        }

        return user;
    }

    public void Save()
    {
        User user = DataSystem.Instance.User ?? CreateUserFile();

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/game.save");
        binaryFormatter.Serialize(file, user);
        file.Close();

        Load();
    }

    public void Load()
    {
        Debug.Log("Game Save is Exist");

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/game.save", FileMode.Open);
        User user = (User)binaryFormatter.Deserialize(file);
        file.Close();

        DataSystem.Instance.User = user;
    }

    public User GetCurrentUser() => DataSystem.Instance.User;

}
