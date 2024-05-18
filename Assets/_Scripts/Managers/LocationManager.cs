using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocationManager : MonoBehaviour
{
    public static bool isExit;
    [SerializeField] public Location location;
    [SerializeField] TextMeshProUGUI TextLocation;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isExit = false;
            TextLocation.text = location.locationName;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isExit = true;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (isExit)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TextLocation.text = location.locationName;
            }
        }
    }
}
