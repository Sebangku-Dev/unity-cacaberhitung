using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardUser : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI userText;

    void Start()
    {
        userText.text = $"Halo, {UserManager.Instance.GetCurrentUser().name ?? "User"}";
    }

}
