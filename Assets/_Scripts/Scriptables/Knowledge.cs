using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Knowledge", menuName ="Scriptable/Knowledge")]
public class Knowledge : ScriptableObject
{
    public int id;
    public string title;
    public string explanation;
    public Sprite image;
    public LevelType.LevelTypes type;
    public AudioClip explanationVoice;
}
