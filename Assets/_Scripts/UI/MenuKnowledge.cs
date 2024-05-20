using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuKnowledge : MonoBehaviour
{
    [SerializeField] GameObject KnowledgeContent, KnowledgeCard, KnowledgeCardEmpty;
    bool isInitCards = true;

    // Update is called once per frame
    void Update()
    {
        if (UserManager.Instance.User != null && DataSystem.Instance.Knowledge != null)
        {
            if (isInitCards)
            {
                GenerateCards();
                isInitCards = false;
            }
        }
    }

    void GenerateCards()
    {
        int index = 0;
        foreach (Knowledge knowledge in DataSystem.Instance.Knowledge)
        {
            if (UserManager.Instance.User.listOfSaveKnowledge[index].isCollected)
            {
                GameObject card = Instantiate(KnowledgeCard, KnowledgeContent.transform);
                CardKnowledge cardKnowledge = card.GetComponent<CardKnowledge>();
                if (cardKnowledge) cardKnowledge.knowledge = knowledge;
            }
            index++;
        }

        KnowledgeCardEmpty.transform.SetAsLastSibling();
    }
}
