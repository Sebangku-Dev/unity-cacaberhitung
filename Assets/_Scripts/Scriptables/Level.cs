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

    [Header("Used in menu or main")]
    public string description;
    public bool isSolved;
    public bool isRightInTime;
    public bool isNoMistake;

    public Sprite levelSprite;
    public Mesh levelMesh;

    [Header("Used in game")]
    public BaseGameplay gameplayLevelPrefab;
}
