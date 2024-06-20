using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    [SerializeField] private Button playButton;

    void Start()
    {
        playButton.onClick.AddListener(() => NavigationSystem.Instance.LoadScene("Main"));
    }
}
