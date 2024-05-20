using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    [SerializeField] private Button playButton;
    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(()=>NavigationSystem.Instance.LoadScene("Main"));
    }
}
