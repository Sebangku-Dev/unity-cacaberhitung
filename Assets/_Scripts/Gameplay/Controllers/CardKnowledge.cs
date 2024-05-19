using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardKnowledge : MonoBehaviour
{
    public Knowledge knowledge;
    bool isLoaded = true;

    [SerializeField] TextMeshProUGUI TextTitle, TextContext, TextContent;
    [SerializeField] Button ButtonVoice;
    // Update is called once per frame
    void Update()
    {
        if (knowledge != null && isLoaded)
        {
            SetContent();
            isLoaded = false;
        }
    }

    void SetContent()
    {
        TextTitle.text = "Tahukah kamu #" + knowledge.id + "?";
        TextContext.text = knowledge.title;
        TextContent.text = knowledge.explanation;
    }

    public void OnPlayKnowledgeAudio()
    {

    }
}
