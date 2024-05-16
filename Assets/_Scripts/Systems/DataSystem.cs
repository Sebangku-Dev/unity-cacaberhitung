using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DataSystem : SingletonPersistent<DataSystem>
{
    public User User { get; private set; }

    public List<Level> Levels { get; private set; }
    public List<Achievement> Achievements { get; private set; }
    public List<Knowledge> Knowledge { get; private set; }
    public TodaysKnowledge currentKnowledge;
    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        Levels = Resources.LoadAll<Level>("Levels").ToList();
        Knowledge = Resources.LoadAll<Knowledge>("Knowledges").ToList();
    }
}
