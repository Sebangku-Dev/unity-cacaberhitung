

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public Level GetReadyToPlayLevel()
    {
        return DataSystem.Instance.Levels.Find(level => level.isToBePlayed && level.isUnlocked) ?? null;
    }

    public Level GetLevelById(int id)
    {
        return DataSystem.Instance.Levels.Find(level => level.id == id);
    }

    public List<Level> GetLevelsByType(LevelTypes levelType)
    {
        return DataSystem.Instance.Levels.Where(level => level.type == levelType && level.isUnlocked).ToList();
    }
}
