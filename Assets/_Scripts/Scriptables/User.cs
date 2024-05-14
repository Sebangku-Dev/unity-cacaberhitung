using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveLevel
{
    public string id;
    public int starCount;
    public string finishAt;
}

[System.Serializable]
public class User
{
    public string id;
    public string name;
    public int age;
    public int currentLevel;
    public List<SaveLevel> listOfSaveLevel;
}
