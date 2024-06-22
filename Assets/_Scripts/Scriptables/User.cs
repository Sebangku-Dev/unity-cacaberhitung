using System.Collections.Generic;
using System;

[System.Serializable]
public class SaveLevel
{
    public int id;
    public bool isUnlocked;
    public bool isSolved;
    public bool isRightInTime;
    public bool isNoMistake;
    public bool isToBePlayed;
    public int playCount;
}

[System.Serializable]
public class TodaysKnowledge
{
    public int id = -1;
    public int areaId;
    public float x;
    public float y;
    public float z;
    public DateTime startingAt;
    public bool isAnswered = false;
}

[System.Serializable]
public class SaveKnowledge
{
    public int id;
    public bool isCollected;
    public string collectedAt;
}

[System.Serializable]
public class SaveAchievement
{
    public int id;
    public bool isUnlocked;
}

[System.Serializable]
public class User
{
    public string id;
    public string name;
    public int age;
    public int currentScore;
    public List<SaveLevel> savedLevels = new List<SaveLevel>();
    public List<SaveKnowledge> savedKnowledge = new List<SaveKnowledge>();
    public List<SaveAchievement> savedAchievement = new List<SaveAchievement>();
    public bool knowledgeHasSpawn;
    public TodaysKnowledge currentKnowledge;
}



