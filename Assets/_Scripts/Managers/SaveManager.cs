using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static User newUser;
    private User CreateUserFile()
    {
        User user = newUser ?? new User();

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

        DataSystem.Instance.User = user;

        return user;
    }
    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/game.save"))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/game.save", FileMode.Open);
            User user = (User)binaryFormatter.Deserialize(file);
            file.Close();

            DataSystem.Instance.User = user;

        }
        else
        {
            Debug.LogWarning("File save isn't exist");
        }
    }

    public void SaveData()
    {
        User user = DataSystem.Instance.User ?? CreateUserFile();

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/game.save");
        binaryFormatter.Serialize(file, user);
        file.Close();
    }
}
