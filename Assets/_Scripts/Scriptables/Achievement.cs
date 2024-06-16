using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable/Achievement")]
public class Achievement : ScriptableObject
{
    public int id;
    public string title;
    public Sprite badge;
    public string description;
    public bool isUnlocked;
    public int pointReward = 0;
    public int energyReward = 0;
}
