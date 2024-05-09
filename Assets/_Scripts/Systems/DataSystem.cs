using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataSystem : SingletonPersistent<DataSystem>
{
    public User User { get; private set; }

    private List<Level> levels;
    private List<Achievement> achievements;
    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        levels = Resources.LoadAll<Level>("Easd").ToList();
    }

    private Level GetLevelById(int id)
    {
        return levels.Find(level => level.id == id);
    }
}
