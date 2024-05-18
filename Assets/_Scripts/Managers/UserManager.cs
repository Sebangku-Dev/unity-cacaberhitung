using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
public class UserManager : Singleton<UserManager>
{
    [SerializeField] bool isLoadData;
    public User User { get; set; }
    public User NewUser;
    void Start()
    {
        if (Instance.User != null)
        {
            if (isLoadData) Instance.Load();
        }
    }

    public User CreateUserFile()
    {
        User user = NewUser ?? new User();

        foreach (Level level in DataSystem.Instance.Levels)
        {
            SaveLevel saved = new()
            {
                id = level.id,
                isFinished = false
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

        Instance.User = user;

        return user;
    }

    public void Save()
    {
        User user = Instance.User ?? CreateUserFile();

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/game.save");
        binaryFormatter.Serialize(file, user);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/game.save"))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/game.save", FileMode.Open);
            User user = (User)binaryFormatter.Deserialize(file);
            file.Close();

            Instance.User = user;

        }
        else
        {
            Debug.LogWarning("File save isn't exist");
        }
    }
}
