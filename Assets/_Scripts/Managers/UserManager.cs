using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;
public class UserManager : Singleton<UserManager>
{

    [Header("Login as Guest")]
    [SerializeField] public User NewUser;

    protected void Start()
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

            user.savedLevels.Add(saved);
        }

        foreach (Knowledge knowledge in DataSystem.Instance.Knowledge)
        {
            SaveKnowledge saved = new()
            {
                id = knowledge.id,
                isCollected = false
            };

            user.savedKnowledge.Add(saved);
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

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/game.save", FileMode.Open);
        User user = (User)binaryFormatter.Deserialize(file);
        file.Close();

        Debug.Log("Game Save is Exist");
        Debug.Log("User: " + JsonUtility.ToJson(user));

        DataSystem.Instance.User = user;
    }

    public void Register(string name, string age)
    {
        UserManager.Instance.NewUser = new User()
        {
            id = name + DateTime.Now.ToString("yyyy-MM-dd"),
            name = name,
            age = Int32.Parse(age),
            currentLevel = 1,
            currentScore = 0
        };

        Save();

        Debug.Log(JsonUtility.ToJson(UserManager.Instance.GetCurrentUser()));
    }

    public User GetCurrentUser() => DataSystem.Instance.User;

}
