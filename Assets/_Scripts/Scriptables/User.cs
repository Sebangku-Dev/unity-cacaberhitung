using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveLevel
{
    public int id;
    public bool isFinished;
    public int starCount;
    public string finishAt;
}

[System.Serializable]
public class SaveKnowledge
{
    public int id;
    public bool isCollected;
    public string collectedAt;
}

[System.Serializable]
public class User
{
    public string id;
    public string name;
    public int age;
    public int currentLevel;
    public List<SaveLevel> listOfSaveLevel = new List<SaveLevel>();
    public List<SaveKnowledge> listOfSaveKnowledge = new List<SaveKnowledge>();
    public TodaysKnowledge currentKnowledge;
}
