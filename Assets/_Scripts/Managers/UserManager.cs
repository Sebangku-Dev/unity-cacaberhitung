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
        if (isLoadData) UserManager.Instance.Load();
    }

    public User CreateUserFile()
    {
        User user = UserManager.Instance.NewUser ?? new User();

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

        return user;
    }

    public void Save()
    {
        User user = UserManager.Instance.User ?? CreateUserFile();

        BinaryFormatter binaryFormatter = new();
        FileStream file = File.Create(Application.persistentDataPath + "/game.save");
        binaryFormatter.Serialize(file, user);
        file.Close();

        Load();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/game.save"))
        {
            Debug.Log("game save exist");
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/game.save", FileMode.Open);
            User user = (User)binaryFormatter.Deserialize(file);
            file.Close();

            UserManager.Instance.User = user;

        }
        else
        {
            Save();
        }
    }
}
