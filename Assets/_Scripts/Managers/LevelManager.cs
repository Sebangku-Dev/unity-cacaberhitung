using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Level currentLevelData;
    public void GetLevelById(int id)
    {
        this.currentLevelData = DataSystem.Instance.Levels.Find(level => level.id == id);
        // return currentLevelData;
    }

    public void ExecuteCurrentLevel()
    {
        // if (GetComponent(currentLevelData.gameplayLevelPrefab.GetType()) == null)
        // {
        //     gameObject.AddComponent(currentLevelData.gameplayLevelPrefab.GetType());
        // }
        if (transform.childCount == 0)
        {
            var levelInstance = Instantiate(currentLevelData.gameplayLevelPrefab, this.transform);
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
