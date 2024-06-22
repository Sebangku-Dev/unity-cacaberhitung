using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementContainer : MonoBehaviour
{
    [SerializeField] AchievementCard achievementCardPrefab;

    void Start()
    {
        ShowAchievements();
    }

    public void ShowAchievements()
    {
        List<Achievement> achievements = AchievementManager.Instance.GetAllAchievements();

        foreach (Achievement achievement in achievements)
        {
            var instantiatedLevelCard = Instantiate(achievementCardPrefab, transform);
            instantiatedLevelCard.imagePlaceholder.sprite = achievement.isUnlocked ? achievement.badge : instantiatedLevelCard.imagePlaceholder.sprite;
        }
        
    }
}
