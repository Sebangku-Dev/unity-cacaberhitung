

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
}
