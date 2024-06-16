using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPoin : MonoBehaviour
{
    [SerializeField] Button poinButton, achievementButton, knowledgeButton;
    // Start is called before the first frame update
    void Start()
    {
        knowledgeButton.onClick.AddListener(()=>NavigationSystem.Instance.LoadScene("MenuKnowledge"));
    }
}
