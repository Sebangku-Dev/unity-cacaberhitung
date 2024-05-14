using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocationManager : MonoBehaviour
{
    [SerializeField] Location location;
    [SerializeField] TextMeshProUGUI TextLocation;
    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            TextLocation.text = location.locationName;
        }
    }
}
