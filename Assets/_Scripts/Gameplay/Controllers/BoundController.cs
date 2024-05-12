using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundController : MonoBehaviour
{
    [SerializeField] bool isInBound;
    [SerializeField] BoundController inBound;
    public bool isPlayerInBound = true;
    [SerializeField] Transform player;
    [SerializeField] Collider bounds;

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isInBound) isPlayerInBound = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isInBound) isPlayerInBound = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isInBound)
        {
            if (!inBound.isPlayerInBound)
            {
                Debug.Log("is out of bound");
                player.GetComponent<PlayerController>().rb.velocity = Vector3.zero;
                player.GetComponent<PlayerController>().rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
