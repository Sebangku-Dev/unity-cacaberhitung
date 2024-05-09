using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    // [SerializeField]
    private Level currentLevel;

    public void ExecuteCurrentLevel()
    {
        if (transform.childCount == 0)
        {
            var levelInstance = Instantiate(currentLevel.gameplayLevelPrefab, this.transform);
        }
    }

    public void DestroyActiveLevel()
    {
        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }
}
