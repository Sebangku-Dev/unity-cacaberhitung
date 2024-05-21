
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelContainer : MonoBehaviour
{
    [SerializeField] LevelTypes levelType;
    [SerializeField] LevelCard levelCardPrefab;
    [SerializeField] Sprite fallbackSprite;

    private void Start()
    {
        ShowLevelsByType();
    }

    public void ShowLevelsByType()
    {
        List<Level> levels = LevelManager.Instance.GetLevelsByType(levelType);

        // Fallback sprite when levels is not exist
        if (levels.Count < 1)
        {
            var instantiatedLevelCard = Instantiate(levelCardPrefab, transform);
        }
        else
        {
            foreach (Level level in levels)
            {
                var instantiatedLevelCard = Instantiate(levelCardPrefab, transform);
                instantiatedLevelCard.imagePlaceholder.sprite = level.levelSprite;
                instantiatedLevelCard.button.onClick.AddListener(() => NavigationSystem.Instance.LoadScene(level.levelSceneName));
            }
        }


    }
}
