using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataSystem : SingletonPersistent<DataSystem>
{
    public List<Level> Levels { get; private set; }
    public List<Achievement> Achievements { get; private set; }
    public List<Knowledge> Knowledge { get; private set; }

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

    public Level GetReadyToPlayLevel()
    {
        return Levels.Find(level => level.isUnlocked && !level.isSolved) ?? null;
    }

    public Level GetLevelById(int id)
    {
        return Levels.Find(level => level.id == id);
    }
}
