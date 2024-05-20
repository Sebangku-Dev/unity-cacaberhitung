using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable/Achievement")]
public class Achievement : ScriptableObject
{
    public string title;
    public Sprite badge;
    public LevelTypes type;
    public string description;
}
