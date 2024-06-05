

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] TextMeshProUGUI MissionTitle, MissionDetail;
    void Start()
    {
        Level CurrentLevel = GetReadyToPlayLevel();

        if (CurrentLevel != null)
        {
            MissionTitle.text = CurrentLevel.title;
            MissionDetail.text = CurrentLevel.hint;
        }
    }
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
