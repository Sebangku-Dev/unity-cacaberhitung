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

    [Header("Used in menu or main")]
    public string description;
    public bool isSolved = false;
    public bool isRightInTime = false;
    public bool isNoMistake = false;

    public int maxTimeDuration = 30;

    public Sprite levelSprite;
    public Mesh levelMesh;

    [Header("Used in game")]
    public string levelSceneName;
}
