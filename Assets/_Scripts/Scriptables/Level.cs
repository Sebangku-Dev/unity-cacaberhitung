using UnityEngine;

public class LevelType
{
    public enum LevelTypes { Counting, Aritmathic, Geometry }
}

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable/Level")]
public class Level : ScriptableObject
{
    [Header("Base information")]
    public int id;
    public string title;
    public LevelType.LevelTypes type;
    public bool isUnlocked = true;

    [Header("Used in menu or main")]
    [TextArea]
    public string description;
    [TextArea]
    public string hint;
    public Sprite levelSprite;


    [Header("Used in game")]
    public string levelSceneName;
    public int playCount = 0;
    public int maxTimeDuration = 30;
    public bool isSolved = false;
    public bool isRightInTime = false;
    public bool isNoMistake = false;
    public Transform marker;
    public Level nextLevel;
}
