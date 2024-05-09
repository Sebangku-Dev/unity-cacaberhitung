using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelType
{
    public enum LevelTypes { Counting, Aritmathic, Geometry }
}

[System.Serializable]
public class QuestionAnswer
{
    public string question;
    public string answer;

}

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable/Level")]
public class Level : ScriptableObject
{
    [Header("Base information")]
    public int id;
    public string title;
    public LevelType.LevelTypes type;

    [Header("Used in menu")]
    public string description;
    public Sprite levelSprite;
    public bool isSolved;

    [Header("Used in game")]
    public BaseGameplay gameplayLevelPrefab;
}
