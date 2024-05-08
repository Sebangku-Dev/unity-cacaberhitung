using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelType
{
    public enum LevelTypes { Counting, Aritmathic, Fraction }
}

[System.Serializable]
public class LevelActivity
{
    public string activityName;
}

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable/Level")]
public class Level : ScriptableObject
{
    public int id;
    public string title;
    public LevelType.LevelTypes type;
    public List<LevelActivity> listActivity;
}
