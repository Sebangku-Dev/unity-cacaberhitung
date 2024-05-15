using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    [TextArea]
    public string question;
    public string[] option;
    public string answer;
}

[CreateAssetMenu(fileName = "Knowledge", menuName = "Scriptable/Knowledge")]
public class Knowledge : ScriptableObject
{
    [Header("Knowledge's general Information")]
    public int id;
    public LevelType.LevelTypes type;
    public string title;
    [TextArea]
    public string explanation;

    [Header("Knowledge's Misc")]
    public Sprite image;
    public AudioClip explanationVoice;

    [Header("Knowledge's Quiz")]
    [SerializeField] public Question question;
}
